namespace Yolo11Sharp.Core
{
    /// <summary>
    /// YOLO11 推理模式
    /// </summary>
    public enum InferenceMode
    {
        /// <summary>
        /// 目标检测 (Detection)
        /// </summary>
        Detection,

        /// <summary>
        /// 图像分类 (Classification)
        /// </summary>
        Classification,

        /// <summary>
        /// 实例分割 (Segmentation)
        /// </summary>
        Segmentation,

        /// <summary>
        /// 定向边界框检测 (Oriented Bounding Box)
        /// </summary>
        OrientedBoundingBox,

        /// <summary>
        /// 姿态估计 (Pose Estimation)
        /// </summary>
        PoseEstimation
    }
} 