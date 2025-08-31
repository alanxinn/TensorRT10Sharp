namespace Yolo11Sharp.Models
{
    /// <summary>
    /// 图像预处理结果
    /// </summary>
    public class PreprocessResult
    {
        /// <summary>
        /// 预处理后的图像数据
        /// </summary>
        public float[] Data { get; set; }

        /// <summary>
        /// X轴缩放比例
        /// </summary>
        public float ScaleX { get; set; }

        /// <summary>
        /// Y轴缩放比例
        /// </summary>
        public float ScaleY { get; set; }

        /// <summary>
        /// X轴填充像素数
        /// </summary>
        public int PadX { get; set; }

        /// <summary>
        /// Y轴填充像素数
        /// </summary>
        public int PadY { get; set; }
    }
} 