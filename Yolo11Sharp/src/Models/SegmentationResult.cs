using System;
using System.Drawing;

namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 实例分割结果
    /// </summary>
    public class SegmentationResult : DetectionResult
    {
        /// <summary>
        /// 分割掩码
        /// </summary>
        public float[,] Mask { get; set; } = new float[0, 0];

        /// <summary>
        /// 掩码宽度
        /// </summary>
        public int MaskWidth => Mask.GetLength(1);

        /// <summary>
        /// 掩码高度
        /// </summary>
        public int MaskHeight => Mask.GetLength(0);

        /// <summary>
        /// 检查结果是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public new bool IsValid()
        {
            return base.IsValid() && Mask.Length > 0;
        }

        /// <summary>
        /// 获取掩码的二值化版本
        /// </summary>
        /// <param name="threshold">二值化阈值</param>
        /// <returns>二值化掩码</returns>
        public bool[,] GetBinaryMask(float threshold = 0.5f)
        {
            var binaryMask = new bool[MaskHeight, MaskWidth];
            for (int y = 0; y < MaskHeight; y++)
            {
                for (int x = 0; x < MaskWidth; x++)
                {
                    binaryMask[y, x] = Mask[y, x] > threshold;
                }
            }
            return binaryMask;
        }

        /// <summary>
        /// 计算掩码面积
        /// </summary>
        /// <param name="threshold">二值化阈值</param>
        /// <returns>掩码面积（像素数）</returns>
        public int GetMaskArea(float threshold = 0.5f)
        {
            int area = 0;
            for (int y = 0; y < MaskHeight; y++)
            {
                for (int x = 0; x < MaskWidth; x++)
                {
                    if (Mask[y, x] > threshold)
                        area++;
                }
            }
            return area;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{ClassName} ({Confidence:F3}): [{X1:F1}, {Y1:F1}, {X2:F1}, {Y2:F1}] Mask: {MaskWidth}x{MaskHeight}";
        }
    }
} 