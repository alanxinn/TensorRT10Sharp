using System;

namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 目标检测结果
    /// </summary>
    public class DetectionResult
    {
        /// <summary>
        /// 边界框左上角X坐标
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// 边界框左上角Y坐标
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// 边界框右下角X坐标
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// 边界框右下角Y坐标
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// 检测置信度
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 边界框宽度
        /// </summary>
        public float Width => X2 - X1;

        /// <summary>
        /// 边界框高度
        /// </summary>
        public float Height => Y2 - Y1;

        /// <summary>
        /// 边界框中心点X坐标
        /// </summary>
        public float CenterX => (X1 + X2) / 2;

        /// <summary>
        /// 边界框中心点Y坐标
        /// </summary>
        public float CenterY => (Y1 + Y2) / 2;

        /// <summary>
        /// 边界框面积
        /// </summary>
        public float Area => Width * Height;

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{ClassName} ({Confidence:F3}): [{X1:F1}, {Y1:F1}, {X2:F1}, {Y2:F1}]";
        }

        /// <summary>
        /// 检查边界框是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return X2 > X1 && Y2 > Y1 && Confidence > 0 && ClassId >= 0;
        }

        /// <summary>
        /// 获取边界框的矩形表示
        /// </summary>
        /// <returns>矩形</returns>
        public System.Drawing.Rectangle ToRectangle()
        {
            return new System.Drawing.Rectangle(
                (int)Math.Max(0, Math.Round(X1)),
                (int)Math.Max(0, Math.Round(Y1)),
                (int)Math.Max(1, Math.Round(Math.Abs(Width))),
                (int)Math.Max(1, Math.Round(Math.Abs(Height)))
            );
        }

        /// <summary>
        /// 获取边界框的矩形F表示
        /// </summary>
        /// <returns>矩形F</returns>
        public System.Drawing.RectangleF ToRectangleF()
        {
            return new System.Drawing.RectangleF(
                Math.Max(0, X1), 
                Math.Max(0, Y1), 
                Math.Max(1, Math.Abs(Width)), 
                Math.Max(1, Math.Abs(Height))
            );
        }
    }
} 