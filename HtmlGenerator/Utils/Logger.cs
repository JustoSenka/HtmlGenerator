using HtmlGenerator.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HtmlGenerator
{
    public static class Logger
    {
        private static ILogger m_UsedLogger;
        public static ILogger UsedLogger
        {
            get
            {
                if (m_UsedLogger == null)
                    m_UsedLogger = new LoggerToFile();

                return m_UsedLogger;
            }
            set
            {
                m_UsedLogger = value;
            }
        }

        public static IList<Log> LogList => UsedLogger.LogList;

        public static void LogError(string message) => UsedLogger.LogError(message);
        public static void LogWarning(string message) => UsedLogger.LogWarning(message);
        public static void LogMessage(string message) => UsedLogger.LogMessage(message);
    }

    namespace Logging
    {
        public interface ILogger
        {
            IList<Log> LogList { get; }

            void LogError(string message);
            void LogWarning(string message);
            void LogMessage(string message);
        }

        public class LoggerToConsole : ILogger
        {
            public IList<Log> LogList { get; } = new List<Log>();

            protected virtual void Log(LogType logType, string msg)
            {
                var log = new Log(logType, msg);

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
            private readonly string LogPathFull;

            public LoggerToFile()
            {
                LogPathFull = Path.Combine(Environment.CurrentDirectory, LogPath);

                if (File.Exists(LogPathFull))
                    File.Delete(LogPathFull);

                try
                {
                    File.Create(LogPathFull).Close();
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
                    File.AppendAllLines(LogPathFull, new[] { str });
                }
                catch (IOException e)
                {
                    Console.WriteLine("Logger failed to append file: " + e.Message);
                }
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
