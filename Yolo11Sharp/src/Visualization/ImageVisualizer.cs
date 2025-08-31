using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Yolo11Sharp.Core;
using Yolo11Sharp.Models;

namespace Yolo11Sharp.Visualization
{
    /// <summary>
    /// 图像可视化工具类
    /// </summary>
    public static class ImageVisualizer
    {
        private static readonly Color[] DefaultColors = new Color[]
        {
            Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange,
            Color.Purple, Color.Cyan, Color.Magenta, Color.Lime, Color.Pink,
            Color.Teal, Color.Lavender, Color.Brown, Color.Beige, Color.Maroon,
            Color.MintCream, Color.Olive, Color.Navy, Color.Aquamarine, Color.Turquoise
        };

        /// <summary>
        /// 在图像上绘制检测结果
        /// </summary>
        /// <param name="imagePath">输入图像路径</param>
        /// <param name="detections">检测结果列表</param>
        /// <param name="outputPath">输出图像路径</param>
        /// <param name="thickness">边界框线条粗细</param>
        /// <param name="fontSize">字体大小</param>
        public static void DrawDetections(string imagePath, List<DetectionResult> detections, 
            string outputPath, int thickness = 2, float fontSize = 12f)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                throw new ArgumentException("输入图像文件不存在", nameof(imagePath));

            if (string.IsNullOrEmpty(outputPath))
                throw new ArgumentNullException(nameof(outputPath));

            using var bitmap = new Bitmap(imagePath);
            var resultBitmap = DrawDetections(bitmap, detections, thickness, fontSize);
            
            // 确保输出目录存在
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            resultBitmap.Save(outputPath, GetImageFormat(outputPath));
            resultBitmap.Dispose();
        }

        /// <summary>
        /// 在Bitmap图像上绘制检测结果
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <param name="detections">检测结果列表</param>
        /// <param name="thickness">边界框线条粗细</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns>绘制结果的新Bitmap</returns>
        public static Bitmap DrawDetections(Bitmap bitmap, List<DetectionResult> detections, 
            int thickness = 2, float fontSize = 12f)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            if (detections == null || detections.Count == 0)
                return new Bitmap(bitmap);

            var result = new Bitmap(bitmap.Width, bitmap.Height);
            using var graphics = Graphics.FromImage(result);
            
            // 设置高质量渲染
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // 绘制原始图像
            graphics.DrawImage(bitmap, 0, 0);

            // 创建字体
            using var font = new Font("Arial", fontSize, FontStyle.Bold);
            using var backgroundBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));

            foreach (var detection in detections)
            {
                if (!detection.IsValid())
                    continue;

                // 获取颜色
                var color = GetColorForClass(detection.ClassId);
                using var pen = new Pen(color, thickness);
                using var brush = new SolidBrush(color);

                // 绘制边界框
                var rect = detection.ToRectangleF();
                
                // 确保边界框在图像范围内
                float x = Math.Max(0, Math.Min(rect.X, bitmap.Width - 1));
                float y = Math.Max(0, Math.Min(rect.Y, bitmap.Height - 1));
                float width = Math.Max(1, Math.Min(rect.Width, bitmap.Width - x));
                float height = Math.Max(1, Math.Min(rect.Height, bitmap.Height - y));
                
                graphics.DrawRectangle(pen, x, y, width, height);

                // 准备标签文本
                var label = $"{detection.ClassName} {detection.Confidence:F2}";
                var textSize = graphics.MeasureString(label, font);

                // 计算标签位置
                var labelX = Math.Max(0, Math.Min(x, bitmap.Width - textSize.Width - 4));
                var labelY = Math.Max(0, y - textSize.Height - 2);

                // 如果标签超出图像顶部，则放在边界框内部
                if (labelY < 0)
                    labelY = y + 2;

                // 确保标签不超出图像右边界
                if (labelX + textSize.Width + 4 > bitmap.Width)
                    labelX = bitmap.Width - textSize.Width - 4;

                var labelRect = new RectangleF(labelX, labelY, textSize.Width + 4, textSize.Height + 2);

                // 绘制标签背景
                graphics.FillRectangle(brush, labelRect);

                // 绘制标签文本
                graphics.DrawString(label, font, Brushes.White, labelX + 2, labelY + 1);
            }

            return result;
        }

        /// <summary>
        /// 批量处理图像并保存结果
        /// </summary>
        /// <param name="detector">YOLO11检测器</param>
        /// <param name="inputDir">输入图像目录</param>
        /// <param name="outputDir">输出图像目录</param>
        /// <param name="imageExtensions">支持的图像扩展名</param>
        /// <param name="thickness">边界框线条粗细</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns>处理的图像数量</returns>
        public static int ProcessBatch(Yolo11Detection detector, string inputDir, string outputDir,
            string[] imageExtensions = null, int thickness = 2, float fontSize = 12f)
        {
            if (detector == null)
                throw new ArgumentNullException(nameof(detector));

            if (string.IsNullOrEmpty(inputDir) || !Directory.Exists(inputDir))
                throw new ArgumentException("输入目录不存在", nameof(inputDir));

            if (string.IsNullOrEmpty(outputDir))
                throw new ArgumentNullException(nameof(outputDir));

            // 默认支持的图像格式
            imageExtensions ??= new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" };

            // 确保输出目录存在
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            int processedCount = 0;
            var files = Directory.GetFiles(inputDir);

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).ToLowerInvariant();
                if (Array.IndexOf(imageExtensions, extension) == -1)
                    continue;

                var fileName = Path.GetFileNameWithoutExtension(file);
                var outputPath = Path.Combine(outputDir, $"{fileName}_detected{extension}");

                Console.WriteLine($"处理图像: {Path.GetFileName(file)}");

                var detections = detector.Infer(file);
                DrawDetections(file, detections, outputPath, thickness, fontSize);

                Console.WriteLine($"检测到 {detections.Count} 个目标，结果保存至: {Path.GetFileName(outputPath)}");
                processedCount++;
            }

            return processedCount;
        }

        /// <summary>
        /// 根据类别ID获取颜色
        /// </summary>
        /// <param name="classId">类别ID</param>
        /// <returns>颜色</returns>
        private static Color GetColorForClass(int classId)
        {
            return DefaultColors[classId % DefaultColors.Length];
        }

        /// <summary>
        /// 根据文件扩展名获取图像格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>图像格式</returns>
        private static ImageFormat GetImageFormat(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".tiff" or ".tif" => ImageFormat.Tiff,
                _ => ImageFormat.Png
            };
        }

        /// <summary>
        /// 打印检测结果统计信息
        /// </summary>
        /// <param name="detections">检测结果列表</param>
        public static void PrintDetectionStats(List<DetectionResult> detections)
        {
            if (detections == null || detections.Count == 0)
            {
                Console.WriteLine("未检测到任何目标");
                return;
            }

            Console.WriteLine($"检测结果统计:");
            Console.WriteLine($"总检测数: {detections.Count}");

            // 按类别统计
            var classStats = new Dictionary<string, int>();
            foreach (var detection in detections)
            {
                var className = detection.ClassName ?? "Unknown";
                classStats[className] = classStats.GetValueOrDefault(className, 0) + 1;
            }

            Console.WriteLine("各类别检测数:");
            foreach (var kvp in classStats)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            // 置信度统计
            var confidences = detections.ConvertAll(d => d.Confidence);
            var avgConfidence = confidences.Sum() / confidences.Count;
            var maxConfidence = confidences.Max();
            var minConfidence = confidences.Min();

            Console.WriteLine($"置信度统计:");
            Console.WriteLine($"  平均: {avgConfidence:F3}");
            Console.WriteLine($"  最高: {maxConfidence:F3}");
            Console.WriteLine($"  最低: {minConfidence:F3}");
        }
    }
} 