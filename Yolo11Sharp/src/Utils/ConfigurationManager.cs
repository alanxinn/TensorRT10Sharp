using System;
using System.IO;

namespace Yolo11Sharp.Utils
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public static class ConfigurationManager
    {
        /// <summary>
        /// 默认配置
        /// </summary>
        public static class Defaults
        {
            public const string EngineFileName = "yolo11n.engine";
            public const string OnnxFileName = "yolo11n.onnx";
            public const string ClassNamesFileName = "coco.names";
            public const string TestImageFileName = "test.jpg";
            public const string OutputImageFileName = "result.jpg";
            public const float DefaultConfidenceThreshold = 0.25f;
            public const float DefaultNmsThreshold = 0.45f;
            public const int DefaultMemorySize = 1024;
        }

        /// <summary>
        /// 获取资源文件路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>完整路径</returns>
        public static string GetAssetPath(string fileName)
        {
            var assetsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            if (!Directory.Exists(assetsDir))
            {
                assetsDir = "Assets";
            }
            return Path.Combine(assetsDir, fileName);
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }

        /// <summary>
        /// 获取输出目录
        /// </summary>
        /// <returns>输出目录路径</returns>
        public static string GetOutputDirectory()
        {
            var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            return outputDir;
        }

        /// <summary>
        /// 获取完整的输出文件路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>完整路径</returns>
        public static string GetOutputPath(string fileName)
        {
            return Path.Combine(GetOutputDirectory(), fileName);
        }
    }
} 