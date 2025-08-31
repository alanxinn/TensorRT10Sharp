using System;
using System.Runtime.InteropServices;

namespace TensorRTSharp
{
    /// <summary>
    /// TensorRT推理引擎类
    /// </summary>
    public class Nvinfer : IDisposable
    {
        private IntPtr _nativePtr;
        private bool _disposed = false;
        
        #region Native Methods
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Nvinfer_Create();
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr Nvinfer_CreateWithModel(string modelPath);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Nvinfer_Destroy(IntPtr ptr);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Dims Nvinfer_GetBindingDimensions(IntPtr ptr, int index);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern Dims Nvinfer_GetBindingDimensionsByName(IntPtr ptr, string nodeName);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void Nvinfer_LoadInferenceDataByName(IntPtr ptr, string nodeName, float[] data, int dataSize);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Nvinfer_LoadInferenceData(IntPtr ptr, int nodeIndex, float[] data, int dataSize);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Nvinfer_Infer(IntPtr ptr);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr Nvinfer_GetInferenceResultByName(IntPtr ptr, string nodeName, out int resultSize);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Nvinfer_GetInferenceResult(IntPtr ptr, int nodeIndex, out int resultSize);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Nvinfer_FreeResult(IntPtr result);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Nvinfer_GetInputCount(IntPtr ptr);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Nvinfer_GetOutputCount(IntPtr ptr);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Nvinfer_GetInputName(IntPtr ptr, int index);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Nvinfer_GetOutputName(IntPtr ptr, int index);
        
        [DllImport("trt10.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern bool OnnxToEngine(string modelPath, int memorySize);
        
        #endregion
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public Nvinfer()
        {
            _nativePtr = Nvinfer_Create();
            if (_nativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create TensorRT inference engine");
            }
        }
        
        /// <summary>
        /// 构造函数，加载模型
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        public Nvinfer(string modelPath)
        {
            if (string.IsNullOrEmpty(modelPath))
            {
                throw new ArgumentNullException(nameof(modelPath));
            }
            
            _nativePtr = Nvinfer_CreateWithModel(modelPath);
            if (_nativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to load TensorRT model: {modelPath}");
            }
        }
        
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Nvinfer()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否正在释放</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && _nativePtr != IntPtr.Zero)
            {
                Nvinfer_Destroy(_nativePtr);
                _nativePtr = IntPtr.Zero;
                _disposed = true;
            }
        }
        
        /// <summary>
        /// 获取绑定端口的形状信息（通过索引）
        /// </summary>
        /// <param name="index">绑定端口的编号</param>
        /// <returns>维度信息</returns>
        public Dims GetBindingDimensions(int index)
        {
            CheckDisposed();
            return Nvinfer_GetBindingDimensions(_nativePtr, index);
        }
        
        /// <summary>
        /// 获取绑定端口的形状信息（通过名称）
        /// </summary>
        /// <param name="nodeName">绑定端口的名称</param>
        /// <returns>维度信息</returns>
        public Dims GetBindingDimensions(string nodeName)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }
            return Nvinfer_GetBindingDimensionsByName(_nativePtr, nodeName);
        }
        
        /// <summary>
        /// 加载待推理数据（通过名称）
        /// </summary>
        /// <param name="nodeName">待加载推理数据端口的名称</param>
        /// <param name="data">处理好的待推理数据</param>
        public void LoadInferenceData(string nodeName, float[] data)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            }
            
            Nvinfer_LoadInferenceDataByName(_nativePtr, nodeName, data, data.Length);
        }
        
        /// <summary>
        /// 加载待推理数据（通过索引）
        /// </summary>
        /// <param name="nodeIndex">待加载推理数据端口的编号</param>
        /// <param name="data">处理好的待推理数据</param>
        public void LoadInferenceData(int nodeIndex, float[] data)
        {
            CheckDisposed();
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            }
            
            Nvinfer_LoadInferenceData(_nativePtr, nodeIndex, data, data.Length);
        }
        
        /// <summary>
        /// 执行推理
        /// </summary>
        public void Infer()
        {
            CheckDisposed();
            Nvinfer_Infer(_nativePtr);
        }
        
        /// <summary>
        /// 获取推理结果（通过名称）
        /// </summary>
        /// <param name="nodeName">推理结果数据端口的名称</param>
        /// <returns>推理结果数据</returns>
        public float[] GetInferenceResult(string nodeName)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }
            
            IntPtr resultPtr = Nvinfer_GetInferenceResultByName(_nativePtr, nodeName, out int resultSize);
            if (resultPtr == IntPtr.Zero || resultSize <= 0)
            {
                return new float[0];
            }
            
            float[] result = new float[resultSize];
            Marshal.Copy(resultPtr, result, 0, resultSize);
            Nvinfer_FreeResult(resultPtr);
            
            return result;
        }
        
        /// <summary>
        /// 获取推理结果（通过索引）
        /// </summary>
        /// <param name="nodeIndex">推理结果数据端口的编号</param>
        /// <returns>推理结果数据</returns>
        public float[] GetInferenceResult(int nodeIndex)
        {
            CheckDisposed();
            
            IntPtr resultPtr = Nvinfer_GetInferenceResult(_nativePtr, nodeIndex, out int resultSize);
            if (resultPtr == IntPtr.Zero || resultSize <= 0)
            {
                return new float[0];
            }
            
            float[] result = new float[resultSize];
            Marshal.Copy(resultPtr, result, 0, resultSize);
            Nvinfer_FreeResult(resultPtr);
            
            return result;
        }
        
        /// <summary>
        /// 获取输入数量
        /// </summary>
        /// <returns>输入数量</returns>
        public int GetInputCount()
        {
            CheckDisposed();
            return Nvinfer_GetInputCount(_nativePtr);
        }
        
        /// <summary>
        /// 获取输出数量
        /// </summary>
        /// <returns>输出数量</returns>
        public int GetOutputCount()
        {
            CheckDisposed();
            return Nvinfer_GetOutputCount(_nativePtr);
        }
        
        /// <summary>
        /// 获取输入名称
        /// </summary>
        /// <param name="index">输入索引</param>
        /// <returns>输入名称</returns>
        public string? GetInputName(int index)
        {
            CheckDisposed();
            IntPtr namePtr = Nvinfer_GetInputName(_nativePtr, index);
            if (namePtr == IntPtr.Zero)
            {
                return null;
            }
            return Marshal.PtrToStringAnsi(namePtr);
        }
        
        /// <summary>
        /// 获取输出名称
        /// </summary>
        /// <param name="index">输出索引</param>
        /// <returns>输出名称</returns>
        public string? GetOutputName(int index)
        {
            CheckDisposed();
            IntPtr namePtr = Nvinfer_GetOutputName(_nativePtr, index);
            if (namePtr == IntPtr.Zero)
            {
                return null;
            }
            return Marshal.PtrToStringAnsi(namePtr);
        }
        
        /// <summary>
        /// 检查对象是否已释放
        /// </summary>
        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Nvinfer));
            }
        }
        
        /// <summary>
        /// 将ONNX模型转换为TensorRT引擎
        /// </summary>
        /// <param name="modelPath">ONNX模型路径</param>
        /// <param name="memorySize">模型转换时分配的内存大小（MB）</param>
        /// <returns>转换是否成功</returns>
        public static bool ConvertOnnxToEngine(string modelPath, int memorySize = 1024)
        {
            if (string.IsNullOrEmpty(modelPath))
            {
                throw new ArgumentNullException(nameof(modelPath));
            }
            if (memorySize <= 0)
            {
                throw new ArgumentException("Memory size must be positive", nameof(memorySize));
            }
            
            return OnnxToEngine(modelPath, memorySize);
        }
    }
} 