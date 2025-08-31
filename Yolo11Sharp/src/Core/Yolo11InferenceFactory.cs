using System;
using System.IO;
using Yolo11Sharp.Models;
using Yolo11Sharp.Utils;

namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 推理工厂
    /// </summary>
    public static class Yolo11InferenceFactory
    {
        /// <summary>
        /// 创建检测推理实例
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>检测推理实例</returns>
        public static Yolo11Detection CreateDetection(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f)
        {
            ValidateEnginePath(enginePath);
            return new Yolo11Detection(enginePath, classNamesPath, confThreshold, nmsThreshold);
        }

        /// <summary>
        /// 创建分类推理实例
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <returns>分类推理实例</returns>
        public static Yolo11Classification CreateClassification(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f)
        {
            ValidateEnginePath(enginePath);
            return new Yolo11Classification(enginePath, classNamesPath, confThreshold);
        }

        /// <summary>
        /// 创建分割推理实例（占位符，待实现）
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>分割推理实例</returns>
        public static IYolo11Inference<SegmentationResult> CreateSegmentation(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f)
        {
            ValidateEnginePath(enginePath);
            throw new NotImplementedException("分割推理功能尚未实现");
        }

        /// <summary>
        /// 创建定向边界框推理实例（占位符，待实现）
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>定向边界框推理实例</returns>
        public static IYolo11Inference<OrientedBoundingBoxResult> CreateOrientedBoundingBox(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f)
        {
            ValidateEnginePath(enginePath);
            throw new NotImplementedException("定向边界框推理功能尚未实现");
        }

        /// <summary>
        /// 创建姿态估计推理实例（占位符，待实现）
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>姿态估计推理实例</returns>
        public static IYolo11Inference<PoseEstimationResult> CreatePoseEstimation(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f)
        {
            ValidateEnginePath(enginePath);
            throw new NotImplementedException("姿态估计推理功能尚未实现");
        }

        /// <summary>
        /// 根据推理模式创建推理实例
        /// </summary>
        /// <param name="mode">推理模式</param>
        /// <param name="enginePath">引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>推理实例</returns>
        public static object CreateInference(InferenceMode mode, string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f)
        {
            return mode switch
            {
                InferenceMode.Detection => CreateDetection(enginePath, classNamesPath, confThreshold, nmsThreshold),
                InferenceMode.Classification => CreateClassification(enginePath, classNamesPath, confThreshold),
                InferenceMode.Segmentation => CreateSegmentation(enginePath, classNamesPath, confThreshold, nmsThreshold),
                InferenceMode.OrientedBoundingBox => CreateOrientedBoundingBox(enginePath, classNamesPath, confThreshold, nmsThreshold),
                InferenceMode.PoseEstimation => CreatePoseEstimation(enginePath, classNamesPath, confThreshold, nmsThreshold),
                _ => throw new ArgumentException($"不支持的推理模式: {mode}")
            };
        }

        /// <summary>
        /// 自动检测推理模式（基于模型输出维度）
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        /// <returns>推理模式</returns>
        public static InferenceMode DetectInferenceMode(string enginePath)
        {
            ValidateEnginePath(enginePath);

            using var engine = new Yolo11Engine(enginePath);
            var outputDims = engine.OutputDimensions;

            // 基于输出维度推断模式
            if (outputDims.nbDims == 2)
            {
                // 2D输出通常是分类 [batch, classes]
                return InferenceMode.Classification;
            }
            else if (outputDims.nbDims == 3)
            {
                int outputSize = outputDims.GetDimension(2);
                
                // 基于输出大小推断
                if (outputSize >= 84 && outputSize <= 100)
                {
                    // 通常是检测 [batch, detections, 4+classes] 或 [batch, detections, 4+classes+masks]
                    return InferenceMode.Detection;
                }
                else if (outputSize > 100)
                {
                    // 可能是分割或姿态估计
                    return InferenceMode.Segmentation;
                }
            }

            Logger.Warning($"无法自动检测推理模式，输出维度: {outputDims.nbDims}，默认使用检测模式");
            return InferenceMode.Detection;
        }

        /// <summary>
        /// 验证引擎文件路径
        /// </summary>
        /// <param name="enginePath">引擎文件路径</param>
        private static void ValidateEnginePath(string enginePath)
        {
            if (string.IsNullOrEmpty(enginePath))
                throw new ArgumentNullException(nameof(enginePath));

            if (!File.Exists(enginePath))
                throw new FileNotFoundException($"引擎文件不存在: {enginePath}");

            if (!enginePath.EndsWith(".engine", StringComparison.OrdinalIgnoreCase))
                Logger.Warning($"引擎文件扩展名不是 .engine: {enginePath}");
        }
    }
} 