using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 关键点
    /// </summary>
    public class Keypoint
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 可见性/置信度
        /// </summary>
        public float Visibility { get; set; }

        /// <summary>
        /// 关键点是否可见
        /// </summary>
        public bool IsVisible => Visibility > 0.5f;

        /// <summary>
        /// 转换为 PointF
        /// </summary>
        /// <returns>PointF</returns>
        public PointF ToPointF() => new PointF(X, Y);

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"({X:F1}, {Y:F1}, {Visibility:F2})";
        }
    }

    /// <summary>
    /// 姿态估计结果
    /// </summary>
    public class PoseEstimationResult : DetectionResult
    {
        /// <summary>
        /// 关键点列表
        /// </summary>
        public List<Keypoint> Keypoints { get; set; } = new List<Keypoint>();

        /// <summary>
        /// 可见关键点数量
        /// </summary>
        public int VisibleKeypointsCount => Keypoints.Count(k => k.IsVisible);

        /// <summary>
        /// 关键点总数
        /// </summary>
        public int TotalKeypointsCount => Keypoints.Count;

        /// <summary>
        /// 姿态置信度（基于可见关键点的平均置信度）
        /// </summary>
        public float PoseConfidence
        {
            get
            {
                var visibleKeypoints = Keypoints.Where(k => k.IsVisible).ToList();
                return visibleKeypoints.Count > 0 ? visibleKeypoints.Average(k => k.Visibility) : 0f;
            }
        }

        /// <summary>
        /// 获取指定索引的关键点
        /// </summary>
        /// <param name="index">关键点索引</param>
        /// <returns>关键点，如果索引无效则返回null</returns>
        public Keypoint? GetKeypoint(int index)
        {
            return index >= 0 && index < Keypoints.Count ? Keypoints[index] : null;
        }

        /// <summary>
        /// 获取可见的关键点
        /// </summary>
        /// <returns>可见关键点列表</returns>
        public List<Keypoint> GetVisibleKeypoints()
        {
            return Keypoints.Where(k => k.IsVisible).ToList();
        }

        /// <summary>
        /// 计算姿态边界框（基于可见关键点）
        /// </summary>
        /// <returns>姿态边界框</returns>
        public RectangleF GetPoseBoundingBox()
        {
            var visibleKeypoints = GetVisibleKeypoints();
            if (visibleKeypoints.Count == 0)
                return RectangleF.Empty;

            float minX = visibleKeypoints.Min(k => k.X);
            float maxX = visibleKeypoints.Max(k => k.X);
            float minY = visibleKeypoints.Min(k => k.Y);
            float maxY = visibleKeypoints.Max(k => k.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 计算两个关键点之间的距离
        /// </summary>
        /// <param name="index1">第一个关键点索引</param>
        /// <param name="index2">第二个关键点索引</param>
        /// <returns>距离，如果任一关键点不可见则返回-1</returns>
        public float GetKeypointDistance(int index1, int index2)
        {
            var kp1 = GetKeypoint(index1);
            var kp2 = GetKeypoint(index2);

            if (kp1 == null || kp2 == null || !kp1.IsVisible || !kp2.IsVisible)
                return -1f;

            float dx = kp1.X - kp2.X;
            float dy = kp1.Y - kp2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 检查结果是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public new bool IsValid()
        {
            return base.IsValid() && Keypoints.Count > 0 && VisibleKeypointsCount > 0;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{ClassName} ({Confidence:F3}): [{X1:F1}, {Y1:F1}, {X2:F1}, {Y2:F1}] Keypoints: {VisibleKeypointsCount}/{TotalKeypointsCount} PoseConf: {PoseConfidence:F3}";
        }
    }

    /// <summary>
    /// COCO 姿态关键点定义（17个关键点）
    /// </summary>
    public static class CocoKeypoints
    {
        public const int Nose = 0;
        public const int LeftEye = 1;
        public const int RightEye = 2;
        public const int LeftEar = 3;
        public const int RightEar = 4;
        public const int LeftShoulder = 5;
        public const int RightShoulder = 6;
        public const int LeftElbow = 7;
        public const int RightElbow = 8;
        public const int LeftWrist = 9;
        public const int RightWrist = 10;
        public const int LeftHip = 11;
        public const int RightHip = 12;
        public const int LeftKnee = 13;
        public const int RightKnee = 14;
        public const int LeftAnkle = 15;
        public const int RightAnkle = 16;

        /// <summary>
        /// 关键点名称
        /// </summary>
        public static readonly string[] Names = new string[]
        {
            "Nose", "LeftEye", "RightEye", "LeftEar", "RightEar",
            "LeftShoulder", "RightShoulder", "LeftElbow", "RightElbow",
            "LeftWrist", "RightWrist", "LeftHip", "RightHip",
            "LeftKnee", "RightKnee", "LeftAnkle", "RightAnkle"
        };

        /// <summary>
        /// 骨骼连接定义（用于绘制骨架）
        /// </summary>
        public static readonly (int, int)[] Skeleton = new (int, int)[]
        {
            (LeftAnkle, LeftKnee), (LeftKnee, LeftHip), (RightAnkle, RightKnee), (RightKnee, RightHip),
            (LeftHip, RightHip), (LeftShoulder, LeftHip), (RightShoulder, RightHip),
            (LeftShoulder, RightShoulder), (LeftShoulder, LeftElbow), (RightShoulder, RightElbow),
            (LeftElbow, LeftWrist), (RightElbow, RightWrist), (LeftEye, RightEye),
            (Nose, LeftEye), (Nose, RightEye), (LeftEye, LeftEar), (RightEye, RightEar)
        };
    }
} 