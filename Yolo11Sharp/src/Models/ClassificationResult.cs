using System;

namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 图像分类结果
    /// </summary>
    public class ClassificationResult
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// 置信度
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 检查结果是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return Confidence > 0 && ClassId >= 0;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{ClassName} ({Confidence:F3})";
        }
    }
} 