using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yolo11Sharp.Models;
using Yolo11Sharp.Utils;

namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 图像分类推理
    /// </summary>
    public class Yolo11Classification : Yolo11InferenceBase<ClassificationResult>
    {
        private readonly string[] _classNames;

        /// <summary>
        /// 推理模式
        /// </summary>
        public override InferenceMode Mode => InferenceMode.Classification;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enginePath">TensorRT引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径（可选）</param>
        /// <param name="confThreshold">置信度阈值</param>
        public Yolo11Classification(string enginePath, string classNamesPath = null, float confThreshold = 0.25f) 
            : base(new Yolo11Engine(enginePath), confThreshold)
        {
            // 加载类别名称
            _classNames = LoadClassNames(classNamesPath);
            
            Logger.Info($"YOLO11 分类模型初始化完成");
            Logger.Info($"类别数量: {_classNames.Length}");
            Logger.Info($"置信度阈值: {ConfidenceThreshold}");
        }

        /// <summary>
        /// 后处理分类结果
        /// </summary>
        /// <param name="rawOutput">原始推理输出</param>
        /// <param name="preprocessResult">预处理结果</param>
        /// <returns>分类结果列表</returns>
        protected override List<ClassificationResult> PostProcess(float[] rawOutput, PreprocessResult preprocessResult)
        {
            var results = new List<ClassificationResult>();

            // 对输出进行 Softmax 归一化
            var softmaxOutput = Softmax(rawOutput);

            // 找到所有超过阈值的类别
            for (int i = 0; i < softmaxOutput.Length && i < _classNames.Length; i++)
            {
                if (softmaxOutput[i] >= ConfidenceThreshold)
                {
                    results.Add(new ClassificationResult
                    {
                        ClassId = i,
                        ClassName = GetClassName(i),
                        Confidence = softmaxOutput[i]
                    });
                }
            }

            // 按置信度降序排列
            results = results.OrderByDescending(r => r.Confidence).ToList();

            Logger.Debug($"分类结果数量: {results.Count}");
            if (results.Count > 0)
            {
                Logger.Debug($"Top-1: {results[0].ClassName} ({results[0].Confidence:F3})");
            }

            return results;
        }

        /// <summary>
        /// Softmax 激活函数
        /// </summary>
        /// <param name="input">输入数组</param>
        /// <returns>Softmax 输出</returns>
        private float[] Softmax(float[] input)
        {
            var output = new float[input.Length];
            
            // 找到最大值以提高数值稳定性
            float maxVal = input.Max();
            
            // 计算指数和
            float sum = 0f;
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (float)Math.Exp(input[i] - maxVal);
                sum += output[i];
            }
            
            // 归一化
            for (int i = 0; i < output.Length; i++)
            {
                output[i] /= sum;
            }
            
            return output;
        }

        /// <summary>
        /// 获取 Top-K 分类结果
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <param name="topK">返回前K个结果</param>
        /// <returns>Top-K 分类结果</returns>
        public List<ClassificationResult> GetTopK(string imagePath, int topK = 5)
        {
            var allResults = Infer(imagePath);
            return allResults.Take(topK).ToList();
        }

        /// <summary>
        /// 加载类别名称
        /// </summary>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <returns>类别名称数组</returns>
        private string[] LoadClassNames(string classNamesPath)
        {
            if (string.IsNullOrEmpty(classNamesPath))
            {
                classNamesPath = ConfigurationManager.GetAssetPath("imagenet_classes.txt");
            }

            if (File.Exists(classNamesPath))
            {
                var names = File.ReadAllLines(classNamesPath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToArray();
                Logger.Info($"从文件加载了 {names.Length} 个类别名称: {classNamesPath}");
                return names;
            }
            else
            {
                Logger.Warning($"类别名称文件不存在: {classNamesPath}，使用默认类别名称");
                // 返回默认的ImageNet类别（1000个类别）
                return Enumerable.Range(0, 1000).Select(i => $"class_{i}").ToArray();
            }
        }

        /// <summary>
        /// 获取类别名称
        /// </summary>
        /// <param name="classId">类别ID</param>
        /// <returns>类别名称</returns>
        private string GetClassName(int classId)
        {
            if (classId >= 0 && classId < _classNames.Length)
                return _classNames[classId];
            return $"unknown_{classId}";
        }
    }
} 