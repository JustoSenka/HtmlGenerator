using HtmlGenerator.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HtmlGenerator
{
    public static class Logger
    {
        public static ILogger UsedLogger = new LoggerToFile();

        public static void LogError(string message) => UsedLogger.LogError(message);
        public static void LogWarning(string message) => UsedLogger.LogWarning(message);
        public static void LogMessage(string message) => UsedLogger.LogMessage(message);
    }

    namespace Logging
    {
        public interface ILogger
        {
            void LogError(string message);
            void LogWarning(string message);
            void LogMessage(string message);
        }

        public class LoggerToConsole : ILogger
        {
            public IList<Log> LogList = new List<Log>();

            protected virtual void Log(LogType logType, string msg)
            {
                var log = new Log(LogType.Message, msg);

                var str = "[" + log.LogType + "] " + log.Message;
                Console.WriteLine(str);
                Debug.WriteLine(str);

                LogList.Add(log);
            }

            public void LogMessage(string message) => Log(LogType.Message, message);
            public void LogWarning(string message) => Log(LogType.Warning, message);
            public void LogError(string message) => Log(LogType.Error, message);
        }

        public class LoggerToFile : LoggerToConsole, ILogger
        {
            private const string LogPath = "Log.txt";

            public LoggerToFile()
            {
                if (File.Exists(LogPath))
                    File.Delete(LogPath);

                try
                {
                    File.Create(LogPath);
                }
                catch (IOException) { LogError("Logger cannot create file"); }
            }

            protected override void Log(LogType logType, string msg)
            {
                var log = new Log(LogType.Message, msg);
                var str = "[" + log.LogType + "] " + log.Message;

                base.Log(logType, msg);

                try
                {
                    File.AppendAllLines(LogPath, new[] { str });
                }
                catch (IOException) { }
            }
        }

        [DebuggerDisplay("[{LogType}] {Message}")]
        public struct Log
        {
            public string Message { get; set; }
            public LogType LogType { get; set; }

            public Log(LogType LogType, string Message)
            {
                this.Message = Message;
                this.LogType = LogType;
            }
        }

        [Flags]
        public enum LogType
        {
            Message,
            Warning,
            Error,
        }
    }
}
