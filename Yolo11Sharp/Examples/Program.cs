using System;
using System.Diagnostics;
using System.IO;
using TensorRTSharp;
using Yolo11Sharp.Core;
using Yolo11Sharp.Models;
using Yolo11Sharp.Utils;
using Yolo11Sharp.Visualization;

namespace Yolo11Sharp.Examples
{
    /// <summary>
    /// YOLO11 目标检测示例程序
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("YOLO11 TensorRT C# 目标检测示例");
            Console.WriteLine("=================================");

            // 配置文件路径
            string enginePath = "yolo11n.engine";
            string testImagePath = "bus.jpg";
            string outputImagePath = "result.jpg";
            string classNamesPath = "coco.names"; // 可选

            // 处理命令行参数
            if (args.Length >= 3)
            {
                enginePath = args[0];
                testImagePath = args[1];
                outputImagePath = args[2];
            }
            else if (args.Length == 1 && args[0].ToLower() == "help")
            {
                ShowUsage();
                return;
            }

            // 检查必需文件
            if (!File.Exists(enginePath))
            {
                Console.WriteLine($"错误: TensorRT引擎文件不存在: {enginePath}");
                
                // 检查是否有对应的ONNX文件
                string onnxPath = Path.ChangeExtension(enginePath, ".onnx");
                if (File.Exists(onnxPath))
                {
                    Console.WriteLine($"发现ONNX模型文件: {onnxPath}");
                    Console.WriteLine("正在尝试转换为TensorRT引擎...");
                    
                    if (ConvertOnnxToEngine(onnxPath, enginePath))
                    {
                        Console.WriteLine("✓ ONNX模型转换成功！");
                    }
                    else
                    {
                        Console.WriteLine("✗ ONNX模型转换失败");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("请确保已将ONNX模型转换为TensorRT引擎文件");
                    Console.WriteLine($"或将ONNX模型文件命名为: {onnxPath}");
                    Console.ReadKey();
                    return;
                }
            }

            if (!File.Exists(testImagePath))
            {
                Console.WriteLine($"错误: 测试图像文件不存在: {testImagePath}");
                Console.WriteLine("请将测试图像文件放在程序目录下");
                Console.ReadKey();
                return;
            }

            RunDetectionExample(enginePath, testImagePath, outputImagePath, classNamesPath);
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 转换ONNX模型为TensorRT引擎
        /// </summary>
        /// <param name="onnxPath">ONNX模型路径</param>
        /// <param name="enginePath">输出引擎路径</param>
        /// <returns>转换是否成功</returns>
        static bool ConvertOnnxToEngine(string onnxPath, string enginePath)
        {
            Console.WriteLine($"开始转换: {onnxPath} -> {enginePath}");
            Console.WriteLine("这可能需要几分钟时间，请耐心等待...");
            
            var stopwatch = Stopwatch.StartNew();
            bool success = Nvinfer.ConvertOnnxToEngine(onnxPath, 1024);
            stopwatch.Stop();
            
            if (success && File.Exists(enginePath))
            {
                Console.WriteLine($"转换完成，耗时: {stopwatch.ElapsedMilliseconds / 1000.0:F1}秒");
                return true;
            }
            else
            {
                Console.WriteLine("转换失败，可能的原因:");
                Console.WriteLine("1. CUDA或TensorRT未正确安装");
                Console.WriteLine("2. GPU内存不足");
                Console.WriteLine("3. ONNX模型文件损坏或不兼容");
                return false;
            }
        }

        /// <summary>
        /// 运行检测示例
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="imagePath">输入图像路径</param>
        /// <param name="outputPath">输出图像路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        static void RunDetectionExample(string enginePath, string imagePath, string outputPath, string classNamesPath)
        {
            Yolo11Detection detector = null;
            
            Console.WriteLine($"\n1. 初始化YOLO11检测器...");
            Console.WriteLine($"   引擎文件: {enginePath}");
            Console.WriteLine($"   类别文件: {(File.Exists(classNamesPath) ? classNamesPath : "使用默认COCO类别")}");

            var initStopwatch = Stopwatch.StartNew();
            
            try
            {
                detector = new Yolo11Detection(enginePath, classNamesPath, confThreshold: 0.25f, nmsThreshold: 0.45f);
                initStopwatch.Stop();
                Console.WriteLine($"   初始化完成，耗时: {initStopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                initStopwatch.Stop();
                Console.WriteLine($"   初始化失败: {ex.Message}");
                
                if (ex.Message.Contains("old deserialization"))
                {
                    Console.WriteLine("\n检测到引擎版本不兼容问题！");
                    Console.WriteLine("解决方案:");
                    Console.WriteLine("1. 删除现有的 .engine 文件");
                    Console.WriteLine("2. 重新运行程序，将自动从 ONNX 文件重新生成引擎");
                    
                    string onnxPath = Path.ChangeExtension(enginePath, ".onnx");
                    if (File.Exists(onnxPath))
                    {
                        Console.WriteLine($"\n是否删除旧引擎文件并重新转换？(y/n)");
                        var key = Console.ReadKey().KeyChar;
                        if (key == 'y' || key == 'Y')
                        {
                            File.Delete(enginePath);
                            Console.WriteLine($"\n已删除: {enginePath}");
                            Console.WriteLine("请重新运行程序进行转换");
                        }
                    }
                }
                
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n2. 执行目标检测...");
            Console.WriteLine($"   输入图像: {imagePath}");

            var detectStopwatch = Stopwatch.StartNew();
            var detections = detector.Infer(imagePath);
            detectStopwatch.Stop();

            Console.WriteLine($"   检测完成，耗时: {detectStopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"   检测到 {detections.Count} 个目标");

            if (detections.Count > 0)
            {
                Console.WriteLine("\n3. 检测结果详情:");
                for (int i = 0; i < detections.Count; i++)
                {
                    var detection = detections[i];
                    Console.WriteLine($"   [{i + 1}] {detection}");
                }

                // 打印统计信息
                Console.WriteLine();
                ImageVisualizer.PrintDetectionStats(detections);

                Console.WriteLine($"\n4. 保存可视化结果...");
                Console.WriteLine($"   输出图像: {outputPath}");

                var visualizeStopwatch = Stopwatch.StartNew();
                ImageVisualizer.DrawDetections(imagePath, detections, outputPath);
                visualizeStopwatch.Stop();

                Console.WriteLine($"   可视化完成，耗时: {visualizeStopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"   结果已保存至: {outputPath}");
            }
            else
            {
                Console.WriteLine("\n3. 未检测到任何目标");
                Console.WriteLine("   提示: 可以尝试降低置信度阈值");
            }

            // 性能统计
            Console.WriteLine($"\n5. 性能统计:");
            Console.WriteLine($"   模型初始化: {initStopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"   目标检测: {detectStopwatch.ElapsedMilliseconds}ms");
            if (detections.Count > 0)
            {
                Console.WriteLine($"   结果可视化: {detectStopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"   总耗时: {initStopwatch.ElapsedMilliseconds + detectStopwatch.ElapsedMilliseconds}ms");
            }

            // 清理资源
            detector?.Dispose();
            Console.WriteLine("\n6. 资源清理完成");
        }

        /// <summary>
        /// 运行分类示例
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="imagePath">测试图像路径</param>
        static void RunClassificationExample(string enginePath, string imagePath)
        {
            Console.WriteLine();
            Console.WriteLine("=== 图像分类示例 ===");

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"测试图像不存在: {imagePath}，跳过分类示例");
                return;
            }

            using var classifier = new Yolo11Classification(enginePath);

            var sw = Stopwatch.StartNew();
            var results = classifier.GetTopK(imagePath, 5); // 获取Top-5结果
            sw.Stop();

            Console.WriteLine($"分类结果 (耗时: {sw.ElapsedMilliseconds} ms):");
            Console.WriteLine($"图像: {imagePath}");
            Console.WriteLine();

            if (results.Count > 0)
            {
                Console.WriteLine("Top-5 分类结果:");
                for (int i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    Console.WriteLine($"  {i + 1}. {result.ClassName} - {result.Confidence:P2}");
                }
            }
            else
            {
                Console.WriteLine("未找到符合置信度阈值的分类结果");
            }
        }

        /// <summary>
        /// 显示使用帮助
        /// </summary>
        static void ShowUsage()
        {
            Console.WriteLine("使用方法:");
            Console.WriteLine("  Yolo11Csharp.exe                           # 使用默认参数运行单张图像检测");
            Console.WriteLine("  Yolo11Csharp.exe <engine> <image> <output> # 指定引擎、输入和输出文件");
            Console.WriteLine("  Yolo11Csharp.exe help                      # 显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("参数说明:");
            Console.WriteLine("  engine    - TensorRT引擎文件路径 (.engine)");
            Console.WriteLine("  image     - 输入图像文件路径");
            Console.WriteLine("  output    - 输出图像文件路径");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  Yolo11Csharp.exe yolo11n.engine test.jpg result.jpg");
            Console.WriteLine();
            Console.WriteLine("注意事项:");
            Console.WriteLine("  1. 如果引擎文件不存在但有对应的ONNX文件，程序会自动转换");
            Console.WriteLine("  2. 确保GPU驱动、CUDA和TensorRT已正确安装");
            Console.WriteLine("  3. 首次转换ONNX模型可能需要几分钟时间");
        }
    }
} 