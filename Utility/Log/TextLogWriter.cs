
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace Ifeng.Utility.Log
{
    public class TextLogWriter: ILogWriter
    {
        private static object _lockObj = new object();

		public TextLogWriter()
		{
            this.LogDirectory = ConfigurationManager.AppSettings["LogPath"] ?? ConfigurationManager.AppSettings["ExceptionLogPath"];
            if (string.IsNullOrEmpty(this.LogDirectory)) this.LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
		}

		public TextLogWriter(string logDirectory)
		{
			this.LogDirectory = logDirectory;
		}

        #region Properties

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public string LogDirectory { get; set; }

        #endregion

		#region ILogWriter 成员

		public void WriteMessage(string logName, string message, params object[] args)
		{
			Write(LogType.Message, logName, message, args);
		}

		public void WriteWarning(string logName, string message, params object[] args)
		{
			Write(LogType.Warning, logName, message, args);
		}

		public void WriteError(string logName, string message, params object[] args)
		{
			Write(LogType.Error, logName, message, args);
		}

		public void WriteException(string logName, Exception ex)
		{
			logName = logName ?? ex.GetType().Name;
			Write(LogType.Exception, logName, ex.ToString());
		}

		public void WriteLog(LogType logType, string logName, string message, params object[] args)
		{
			Write(logType, logName, message, args);
		}

		#endregion

		#region Private

		private void Write(LogType logType, string logName, string msg, params object[] args)
		{
            try
            {
                if (System.Threading.Monitor.TryEnter(_lockObj))
                {
                    string filePath = GetLogFilePath(logName ?? logType.ToString());
                    using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        sw.WriteLine("------------------------------------------");
                        sw.WriteLine("LogType: {0}", logType);
                        if (args == null || args.Length == 0) sw.WriteLine(msg);
                        else sw.WriteLine(msg, args);
                        sw.WriteLine();
                    }
                    System.Threading.Monitor.Exit(_lockObj);
                }
            }
            catch
            {
                System.Threading.Monitor.Exit(_lockObj);
            }
		}

		// 获取日志文件路径
		private string GetLogFilePath(string logName)
		{
			string dir = Path.Combine(this.LogDirectory, DateTime.Now.ToString("yyyy-MM-dd"));
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

			return Path.Combine(dir, logName) + ".log";
		}

		#endregion

	}
}
