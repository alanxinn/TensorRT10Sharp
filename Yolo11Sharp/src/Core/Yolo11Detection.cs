using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yolo11Sharp.Models;
using Yolo11Sharp.Utils;

namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 目标检测推理
    /// </summary>
    public class Yolo11Detection : Yolo11InferenceBase<DetectionResult>
    {
        private readonly string[] _classNames;
        private readonly float _nmsThreshold;

        /// <summary>
        /// 推理模式
        /// </summary>
        public override InferenceMode Mode => InferenceMode.Detection;

        /// <summary>
        /// NMS阈值
        /// </summary>
        public float NmsThreshold => _nmsThreshold;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enginePath">TensorRT引擎文件路径</param>
        /// <param name="classNamesPath">类别名称文件路径（可选）</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        public Yolo11Detection(string enginePath, string classNamesPath = null, 
            float confThreshold = 0.25f, float nmsThreshold = 0.45f) 
            : base(new Yolo11Engine(enginePath), confThreshold)
        {
            _nmsThreshold = nmsThreshold;

            // 加载类别名称
            _classNames = LoadClassNames(classNamesPath);
            
            Logger.Info($"YOLO11 检测模型初始化完成");
            Logger.Info($"类别数量: {_classNames.Length}");
            Logger.Info($"置信度阈值: {ConfidenceThreshold}");
            Logger.Info($"NMS阈值: {_nmsThreshold}");
        }

        /// <summary>
        /// 后处理检测结果
        /// </summary>
        /// <param name="rawOutput">原始推理输出</param>
        /// <param name="preprocessResult">预处理结果</param>
        /// <returns>检测结果列表</returns>
        protected override List<DetectionResult> PostProcess(float[] rawOutput, PreprocessResult preprocessResult)
        {
            var detections = new List<DetectionResult>();
            var outputDims = _engine.OutputDimensions;

            // 解析输出维度 - YOLO11 输出格式: [1, 84, 8400]
            int batchSize = outputDims.GetDimension(0);      // 1
            int outputChannels = outputDims.GetDimension(1); // 84 (4坐标 + 80类别)
            int numDetections = outputDims.GetDimension(2);  // 8400

            Logger.Debug($"输出维度: [{batchSize}, {outputChannels}, {numDetections}]");

            // 解析检测结果
            for (int i = 0; i < numDetections; i++)
            {
                // YOLO11 输出格式: [1, 84, 8400]
                // 对于第i个检测框，第j个通道的索引是: j * numDetections + i
                
                // 提取边界框坐标 (中心点格式)
                float centerX = rawOutput[0 * numDetections + i];
                float centerY = rawOutput[1 * numDetections + i];
                float width = rawOutput[2 * numDetections + i];
                float height = rawOutput[3 * numDetections + i];

                // 转换为左上角格式
                float x1 = centerX - width / 2;
                float y1 = centerY - height / 2;
                float x2 = centerX + width / 2;
                float y2 = centerY + height / 2;

                // 获取最高置信度的类别
                int bestClassId = -1;
                float maxConfidence = 0f;

                for (int classId = 0; classId < 80; classId++) // COCO 80个类别
                {
                    int classIndex = (4 + classId) * numDetections + i;
                    if (classIndex >= rawOutput.Length)
                        break;
                        
                    float confidence = rawOutput[classIndex];
                    if (confidence > maxConfidence)
                    {
                        maxConfidence = confidence;
                        bestClassId = classId;
                    }
                }

                // 过滤低置信度检测
                if (maxConfidence < ConfidenceThreshold)
                    continue;

                // 坐标转换回原图尺寸
                var scaleX = preprocessResult.ScaleX;
                var scaleY = preprocessResult.ScaleY;
                var padX = preprocessResult.PadX;
                var padY = preprocessResult.PadY;

                // 1. 先减去填充偏移
                // 2. 再乘以缩放比例转换回原图坐标系
                x1 = (x1 - padX) * scaleX;
                y1 = (y1 - padY) * scaleY;
                x2 = (x2 - padX) * scaleX;
                y2 = (y2 - padY) * scaleY;

                detections.Add(new DetectionResult
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Confidence = maxConfidence,
                    ClassId = bestClassId,
                    ClassName = GetClassName(bestClassId)
                });
            }

            Logger.Debug($"过滤前检测数量: {detections.Count}");

            // 应用NMS
            var nmsResults = ApplyNMS(detections, _nmsThreshold);
            
            Logger.Debug($"NMS后检测数量: {nmsResults.Count}");

            return nmsResults;
        }

        /// <summary>
        /// 应用非最大抑制 (NMS)
        /// </summary>
        /// <param name="detections">检测结果列表</param>
        /// <param name="nmsThreshold">NMS阈值</param>
        /// <returns>NMS后的检测结果</returns>
        private List<DetectionResult> ApplyNMS(List<DetectionResult> detections, float nmsThreshold)
        {
            var result = new List<DetectionResult>();
            var sortedDetections = detections.OrderByDescending(d => d.Confidence).ToList();

            while (sortedDetections.Count > 0)
            {
                var current = sortedDetections[0];
                result.Add(current);
                sortedDetections.RemoveAt(0);

                // 移除与当前检测重叠度高的其他检测
                sortedDetections.RemoveAll(detection => 
                    CalculateIoU(current, detection) > nmsThreshold);
            }

            return result;
        }

        /// <summary>
        /// 计算两个检测结果的IoU (Intersection over Union)
        /// </summary>
        /// <param name="a">检测结果A</param>
        /// <param name="b">检测结果B</param>
        /// <returns>IoU值</returns>
        private float CalculateIoU(DetectionResult a, DetectionResult b)
        {
            // 计算交集区域
            float intersectionX1 = Math.Max(a.X1, b.X1);
            float intersectionY1 = Math.Max(a.Y1, b.Y1);
            float intersectionX2 = Math.Min(a.X2, b.X2);
            float intersectionY2 = Math.Min(a.Y2, b.Y2);

            if (intersectionX2 <= intersectionX1 || intersectionY2 <= intersectionY1)
                return 0f;

            float intersectionArea = (intersectionX2 - intersectionX1) * (intersectionY2 - intersectionY1);
            float unionArea = a.Area + b.Area - intersectionArea;

            return unionArea > 0 ? intersectionArea / unionArea : 0f;
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
                classNamesPath = ConfigurationManager.GetAssetPath(ConfigurationManager.Defaults.ClassNamesFileName);
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
                // 返回默认的COCO类别（80个类别）
                return Enumerable.Range(0, 80).Select(i => $"class_{i}").ToArray();
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