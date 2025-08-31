using System;
using System.Drawing;

namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 定向边界框检测结果
    /// </summary>
    public class OrientedBoundingBoxResult
    {
        /// <summary>
        /// 中心点X坐标
        /// </summary>
        public float CenterX { get; set; }

        /// <summary>
        /// 中心点Y坐标
        /// </summary>
        public float CenterY { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// 旋转角度（弧度）
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// 置信度
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// 旋转角度（度数）
        /// </summary>
        public float AngleDegrees => Angle * 180.0f / (float)Math.PI;

        /// <summary>
        /// 边界框面积
        /// </summary>
        public float Area => Width * Height;

        /// <summary>
        /// 获取四个角点坐标
        /// </summary>
        /// <returns>四个角点坐标数组</returns>
        public PointF[] GetCornerPoints()
        {
            float cos = (float)Math.Cos(Angle);
            float sin = (float)Math.Sin(Angle);
            float halfWidth = Width / 2;
            float halfHeight = Height / 2;

            var corners = new PointF[4];
            
            // 计算四个角点相对于中心的偏移
            var offsets = new PointF[]
            {
                new PointF(-halfWidth, -halfHeight), // 左上
                new PointF(halfWidth, -halfHeight),  // 右上
                new PointF(halfWidth, halfHeight),   // 右下
                new PointF(-halfWidth, halfHeight)   // 左下
            };

            // 应用旋转变换
            for (int i = 0; i < 4; i++)
            {
                float x = offsets[i].X * cos - offsets[i].Y * sin;
                float y = offsets[i].X * sin + offsets[i].Y * cos;
                corners[i] = new PointF(CenterX + x, CenterY + y);
            }

            return corners;
        }

        /// <summary>
        /// 获取轴对齐边界框
        /// </summary>
        /// <returns>轴对齐边界框</returns>
        public RectangleF GetAxisAlignedBoundingBox()
        {
            var corners = GetCornerPoints();
            
            float minX = corners[0].X;
            float maxX = corners[0].X;
            float minY = corners[0].Y;
            float maxY = corners[0].Y;

            for (int i = 1; i < corners.Length; i++)
            {
                minX = Math.Min(minX, corners[i].X);
                maxX = Math.Max(maxX, corners[i].X);
                minY = Math.Min(minY, corners[i].Y);
                maxY = Math.Max(maxY, corners[i].Y);
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 检查结果是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return Width > 0 && Height > 0 && Confidence > 0 && ClassId >= 0;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{ClassName} ({Confidence:F3}): Center({CenterX:F1}, {CenterY:F1}) Size({Width:F1}x{Height:F1}) Angle({AngleDegrees:F1}°)";
        }
    }
} 