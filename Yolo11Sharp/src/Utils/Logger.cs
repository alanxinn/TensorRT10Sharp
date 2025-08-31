using System;

namespace Yolo11Sharp.Utils
{
    /// <summary>
    /// 简单的日志工具
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// 当前日志级别
        /// </summary>
        public static LogLevel CurrentLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">消息</param>
        public static void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message">消息</param>
        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message">消息</param>
        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="message">消息</param>
        public static void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="exception">异常</param>
        public static void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, $"{message}: {exception.Message}");
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="message">消息</param>
        private static void Log(LogLevel level, string message)
        {
            if (level < CurrentLevel)
                return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var levelStr = level.ToString().ToUpper().PadRight(7);
            
            var color = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{timestamp}] [{levelStr}] {message}");
            Console.ForegroundColor = originalColor;
        }
    }
} 