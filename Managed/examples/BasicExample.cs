using System;
using System.Diagnostics;
using TensorRTSharp;

namespace TensorRTSharp.Examples
{
    /// <summary>
    /// TensorRT C# API 基础示例程序
    /// </summary>
    public class BasicExample
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("TensorRT10Sharp C#  测试程序");
            Console.WriteLine("=======================");
            
            var example = new BasicExample();
            example.RunAllTests();
            
            Console.WriteLine("\n测试完成！按任意键退出...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// 运行所有测试
        /// </summary>
        public void RunAllTests()
        {
            try
            {
                // 测试1: ONNX模型转换为TensorRT引擎
                Console.WriteLine("\n1. 测试ONNX模型转换为TensorRT引擎");
                TestOnnxToEngine();
                
                // 测试2: 加载TensorRT引擎并进行推理
                Console.WriteLine("\n2. 测试加载TensorRT引擎并进行推理");
                TestInference();
                
                // 测试3: Dims结构体功能
                Console.WriteLine("\n3. 测试Dims结构体功能");
                TestDims();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 发生错误: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 测试ONNX模型转换
        /// </summary>
        private void TestOnnxToEngine()
        {
            string onnxModelPath = "yolo11n.onnx";
            
            if (System.IO.File.Exists(onnxModelPath))
            {
                Console.WriteLine($"找到ONNX模型文件: {onnxModelPath}");
                bool success = Nvinfer.ConvertOnnxToEngine(onnxModelPath, 1024);
                
                if (success)
                {
                    Console.WriteLine($"✓ 成功将 {onnxModelPath} 转换为 TensorRT 引擎");
                }
                else
                {
                    Console.WriteLine($"✗ 转换 {onnxModelPath} 失败");
                }
            }
            else
            {
                Console.WriteLine($"✗ ONNX模型文件 {onnxModelPath} 不存在");
                Console.WriteLine("请将ONNX模型文件放在项目根目录中");
            }
        }
        
        /// <summary>
        /// 测试推理功能
        /// </summary>
        private void TestInference()
        {
            string enginePath = "yolo11n.engine";
            
            if (!System.IO.File.Exists(enginePath))
            {
                Console.WriteLine($"✗ TensorRT引擎文件 {enginePath} 不存在");
                return;
            }
            
            try
            {
                using var infer = new Nvinfer(enginePath);
                
                // 获取模型信息
                Console.WriteLine($"✓ 成功加载TensorRT引擎: {enginePath}");
                Console.WriteLine($"输入数量: {infer.GetInputCount()}");
                Console.WriteLine($"输出数量: {infer.GetOutputCount()}");
                
                // 显示输入输出信息
                DisplayModelInfo(infer);
                
                // 执行推理测试
                PerformInference(infer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 推理测试失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 显示模型信息
        /// </summary>
        /// <param name="infer">推理引擎</param>
        private void DisplayModelInfo(Nvinfer infer)
        {
            // 获取输入信息
            for (int i = 0; i < infer.GetInputCount(); i++)
            {
                string? inputName = infer.GetInputName(i);
                if (inputName != null)
                {
                    Dims inputDims = infer.GetBindingDimensions(inputName);
                    Console.WriteLine($"输入 {i}: {inputName}, 维度: {FormatDims(inputDims)}");
                }
                else
                {
                    Console.WriteLine($"输入 {i}: null");
                }
            }
            
            // 获取输出信息
            for (int i = 0; i < infer.GetOutputCount(); i++)
            {
                string? outputName = infer.GetOutputName(i);
                if (outputName != null)
                {
                    Dims outputDims = infer.GetBindingDimensions(outputName);
                    Console.WriteLine($"输出 {i}: {outputName}, 维度: {FormatDims(outputDims)}");
                }
                else
                {
                    Console.WriteLine($"输出 {i}: null");
                }
            }
        }
        
        /// <summary>
        /// 执行推理
        /// </summary>
        /// <param name="infer">推理引擎</param>
        private void PerformInference(Nvinfer infer)
        {
            // 准备输入数据
            string? firstInputName = infer.GetInputName(0);
            if (firstInputName == null)
            {
                Console.WriteLine("✗ 无法获取第一个输入名称");
                return;
            }
            
            Dims firstInputDims = infer.GetBindingDimensions(firstInputName);
            int inputSize = firstInputDims.GetElementCount();
            
            float[] inputData = new float[inputSize];
            for (int i = 0; i < inputSize; i++)
            {
                inputData[i] = (float)i / inputSize; // 示例数据
            }
            
            // 加载输入数据
            infer.LoadInferenceData(firstInputName, inputData);
            
            // 执行推理
            Console.WriteLine("执行推理...");
            var stopwatch = Stopwatch.StartNew();
            infer.Infer();
            stopwatch.Stop();
            Console.WriteLine($"✓ 推理完成，耗时: {stopwatch.ElapsedMilliseconds}ms");
            
            // 获取输出结果
            string? firstOutputName = infer.GetOutputName(0);
            if (firstOutputName == null)
            {
                Console.WriteLine("✗ 无法获取第一个输出名称");
                return;
            }
            
            float[] outputData = infer.GetInferenceResult(firstOutputName);
            
            Console.WriteLine($"输出数据大小: {outputData.Length}");
            Console.WriteLine("前10个输出值:");
            for (int i = 0; i < Math.Min(10, outputData.Length); i++)
            {
                Console.Write($"{outputData[i]:F4} ");
            }
            Console.WriteLine();
        }
        
        /// <summary>
        /// 测试Dims结构体功能
        /// </summary>
        private void TestDims()
        {
            try
            {
                // 创建Dims结构体
                Dims dims = new Dims();
                Console.WriteLine($"创建Dims: nbDims={dims.nbDims}");
                
                // 设置维度
                dims.SetDimension(0, 1);
                dims.SetDimension(1, 3);
                dims.SetDimension(2, 640);
                dims.SetDimension(3, 640);
                
                Console.WriteLine($"设置维度后: nbDims={dims.nbDims}");
                Console.WriteLine($"维度值: [{dims.d[0]}, {dims.d[1]}, {dims.d[2]}, {dims.d[3]}]");
                
                // 获取维度信息
                int dim0 = dims.GetDimension(0);
                int dim1 = dims.GetDimension(1);
                Console.WriteLine($"获取维度: dim0={dim0}, dim1={dim1}");
                
                // 计算元素总数
                int elementCount = dims.GetElementCount();
                Console.WriteLine($"元素总数: {elementCount}");
                
                Console.WriteLine("✓ Dims结构体功能测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Dims结构体测试失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 格式化维度信息
        /// </summary>
        /// <param name="dims">维度结构体</param>
        /// <returns>格式化的维度字符串</returns>
        private static string FormatDims(Dims dims)
        {
            if (dims.nbDims == 0)
            {
                return "[]";
            }
            
            string result = "[";
            for (int i = 0; i < dims.nbDims; i++)
            {
                if (i > 0) result += ", ";
                result += dims.d[i];
            }
            result += "]";
            return result;
        }
    }
} 