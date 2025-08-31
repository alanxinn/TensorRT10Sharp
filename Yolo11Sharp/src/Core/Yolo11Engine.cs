using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TensorRTSharp;
using Yolo11Sharp.Models;
using Yolo11Sharp.Utils;

namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 通用推理引擎
    /// </summary>
    public class Yolo11Engine : IDisposable
    {
        private readonly Nvinfer _inferEngine;
        private readonly string _inputName;
        private readonly string _outputName;
        private readonly Dims _inputDims;
        private readonly Dims _outputDims;
        private readonly int _inputWidth;
        private readonly int _inputHeight;
        private readonly int _inputChannels;
        private bool _disposed = false;

        /// <summary>
        /// 模型输入尺寸
        /// </summary>
        public (int Width, int Height) InputSize => (_inputWidth, _inputHeight);

        /// <summary>
        /// 模型输出维度
        /// </summary>
        public Dims OutputDimensions => _outputDims;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enginePath">TensorRT引擎文件路径</param>
        public Yolo11Engine(string enginePath)
        {
            if (string.IsNullOrEmpty(enginePath))
                throw new ArgumentNullException(nameof(enginePath));

            // 初始化推理引擎
            _inferEngine = new Nvinfer(enginePath);

            // 获取输入输出信息
            if (_inferEngine.GetInputCount() == 0 || _inferEngine.GetOutputCount() == 0)
                throw new InvalidOperationException("模型必须至少有一个输入和一个输出");

            _inputName = _inferEngine.GetInputName(0);
            _outputName = _inferEngine.GetOutputName(0);

            if (string.IsNullOrEmpty(_inputName) || string.IsNullOrEmpty(_outputName))
                throw new InvalidOperationException("无法获取输入或输出节点名称");

            _inputDims = _inferEngine.GetBindingDimensions(_inputName);
            _outputDims = _inferEngine.GetBindingDimensions(_outputName);

            // 解析输入维度 (通常为 [batch, channels, height, width])
            if (_inputDims.nbDims != 4)
                throw new InvalidOperationException($"期望输入维度为4，实际为{_inputDims.nbDims}");

            _inputChannels = _inputDims.GetDimension(1);
            _inputHeight = _inputDims.GetDimension(2);
            _inputWidth = _inputDims.GetDimension(3);

            Logger.Info($"模型输入尺寸: {_inputWidth}x{_inputHeight}x{_inputChannels}");
            Logger.Info($"模型输出维度: [{string.Join(", ", Enumerable.Range(0, _outputDims.nbDims).Select(i => _outputDims.GetDimension(i)))}]");
        }

        /// <summary>
        /// 图像预处理
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <returns>预处理结果</returns>
        public PreprocessResult Preprocess(Bitmap bitmap)
        {
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;

            // 计算缩放比例，保持宽高比
            float scale = Math.Min((float)_inputWidth / originalWidth, (float)_inputHeight / originalHeight);
            int newWidth = (int)(originalWidth * scale);
            int newHeight = (int)(originalHeight * scale);

            // 计算填充
            int padX = (_inputWidth - newWidth) / 2;
            int padY = (_inputHeight - newHeight) / 2;

            // 创建目标图像
            using var resizedBitmap = new Bitmap(_inputWidth, _inputHeight);
            using var graphics = Graphics.FromImage(resizedBitmap);
            
            // 填充灰色背景
            graphics.Clear(Color.FromArgb(114, 114, 114));
            
            // 绘制缩放后的图像
            var destRect = new Rectangle(padX, padY, newWidth, newHeight);
            var srcRect = new Rectangle(0, 0, originalWidth, originalHeight);
            graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);

            // 转换为float数组 (CHW格式，归一化到0-1)
            float[] inputData = new float[_inputChannels * _inputHeight * _inputWidth];
            BitmapData bmpData = resizedBitmap.LockBits(
                new Rectangle(0, 0, _inputWidth, _inputHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int stride = bmpData.Stride;

                for (int y = 0; y < _inputHeight; y++)
                {
                    for (int x = 0; x < _inputWidth; x++)
                    {
                        byte* pixel = ptr + y * stride + x * 3;
                        
                        // BGR to RGB 并归一化
                        inputData[0 * _inputHeight * _inputWidth + y * _inputWidth + x] = pixel[2] / 255.0f; // R
                        inputData[1 * _inputHeight * _inputWidth + y * _inputWidth + x] = pixel[1] / 255.0f; // G
                        inputData[2 * _inputHeight * _inputWidth + y * _inputWidth + x] = pixel[0] / 255.0f; // B
                    }
                }
            }

            resizedBitmap.UnlockBits(bmpData);

            Logger.Debug($"原图尺寸: {originalWidth}x{originalHeight}");
            Logger.Debug($"模型输入尺寸: {_inputWidth}x{_inputHeight}");
            Logger.Debug($"缩放比例: {1.0f / scale:F3}");
            Logger.Debug($"填充: ({padX}, {padY})");

            return new PreprocessResult
            {
                Data = inputData,
                ScaleX = 1.0f / scale,  // 从模型坐标转回原图坐标的缩放比例
                ScaleY = 1.0f / scale,  // 保持宽高比，所以X和Y使用相同的缩放比例
                PadX = padX,
                PadY = padY
            };
        }

        /// <summary>
        /// 执行推理
        /// </summary>
        /// <param name="inputData">预处理后的输入数据</param>
        /// <returns>原始推理输出</returns>
        public float[] Infer(float[] inputData)
        {
            CheckDisposed();

            // 执行推理
            _inferEngine.LoadInferenceData(_inputName, inputData);
            _inferEngine.Infer();

            // 获取输出结果
            return _inferEngine.GetInferenceResult(_outputName);
        }

        /// <summary>
        /// 检查对象是否已释放
        /// </summary>
        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Yolo11Engine));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _inferEngine?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 