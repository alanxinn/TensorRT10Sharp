using System;
using System.Collections.Generic;
using System.Drawing;
using Yolo11Sharp.Models;

namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 推理基础接口
    /// </summary>
    /// <typeparam name="TResult">推理结果类型</typeparam>
    public interface IYolo11Inference<TResult> : IDisposable
    {
        /// <summary>
        /// 推理模式
        /// </summary>
        InferenceMode Mode { get; }

        /// <summary>
        /// 模型输入尺寸
        /// </summary>
        (int Width, int Height) InputSize { get; }

        /// <summary>
        /// 置信度阈值
        /// </summary>
        float ConfidenceThreshold { get; set; }

        /// <summary>
        /// 对图像文件进行推理
        /// </summary>
        /// <param name="imagePath">图像文件路径</param>
        /// <returns>推理结果</returns>
        List<TResult> Infer(string imagePath);

        /// <summary>
        /// 对 Bitmap 图像进行推理
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <returns>推理结果</returns>
        List<TResult> Infer(Bitmap bitmap);
    }

    /// <summary>
    /// YOLO11 推理基础抽象类
    /// </summary>
    /// <typeparam name="TResult">推理结果类型</typeparam>
    public abstract class Yolo11InferenceBase<TResult> : IYolo11Inference<TResult>
    {
        protected readonly Yolo11Engine _engine;
        protected bool _disposed = false;

        /// <summary>
        /// 推理模式
        /// </summary>
        public abstract InferenceMode Mode { get; }

        /// <summary>
        /// 模型输入尺寸
        /// </summary>
        public (int Width, int Height) InputSize => _engine.InputSize;

        /// <summary>
        /// 置信度阈值
        /// </summary>
        public float ConfidenceThreshold { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="engine">YOLO11 引擎</param>
        /// <param name="confidenceThreshold">置信度阈值</param>
        protected Yolo11InferenceBase(Yolo11Engine engine, float confidenceThreshold = 0.25f)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            ConfidenceThreshold = confidenceThreshold;
        }

        /// <summary>
        /// 对图像文件进行推理
        /// </summary>
        /// <param name="imagePath">图像文件路径</param>
        /// <returns>推理结果</returns>
        public List<TResult> Infer(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException(nameof(imagePath));

            using var bitmap = new Bitmap(imagePath);
            return Infer(bitmap);
        }

        /// <summary>
        /// 对 Bitmap 图像进行推理
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <returns>推理结果</returns>
        public List<TResult> Infer(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            CheckDisposed();

            // 预处理
            var preprocessResult = _engine.Preprocess(bitmap);

            // 推理
            var rawOutput = _engine.Infer(preprocessResult.Data);

            // 后处理
            return PostProcess(rawOutput, preprocessResult);
        }

        /// <summary>
        /// 后处理抽象方法，由具体实现类重写
        /// </summary>
        /// <param name="rawOutput">原始推理输出</param>
        /// <param name="preprocessResult">预处理结果</param>
        /// <returns>处理后的结果</returns>
        protected abstract List<TResult> PostProcess(float[] rawOutput, PreprocessResult preprocessResult);

        /// <summary>
        /// 检查对象是否已释放
        /// </summary>
        protected void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _engine?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 