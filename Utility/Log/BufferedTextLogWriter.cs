
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace Ifeng.Utility.Log
{
    public class BufferedTextLogWriter: ILogWriter, IDisposable
    {
		private int _BufferLength = 1024 * 10;
		private bool _Disposed;
		private StringBuilder _Buffer = new StringBuilder();
		private readonly object _SyncObj = new object();

		/// <summary>
		/// 日志文件目录
		/// </summary>
		public string LogDirectory { get; set; }
		/// <summary>
		/// 日志文件名
		/// </summary>
		public string LogName { get; set; }
		/// <summary>
		/// Buffer容量
		/// </summary>
		public int BufferLength { get { return _BufferLength; } set { _BufferLength = value; } }

		#region Constructor

		public BufferedTextLogWriter(string logName)
		{
            this.LogDirectory = ConfigurationManager.AppSettings["LogPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
			this.LogName = LogName;
		}

		public BufferedTextLogWriter(string logDirectory, string logName)
        {
            this.LogDirectory = logDirectory;
			this.LogName = logName;
        }

		~BufferedTextLogWriter()
		{
			// 释放非托管资源
			Dispose(false);
		}

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

		#region IDisposable 成员

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_Disposed)
			{
				if (disposing)
				{
					// Release managed resources					
				}

				// Release unmanaged resources
				// 

				Flush();

				// 避免再次释放
				_Disposed = true;
			}
		}

		#endregion

		#region Private

		private void Write(LogType logType, string logName, string msg, params object[] args)
		{
			if (_Disposed) throw new ObjectDisposedException(typeof(BufferedTextLogWriter).Name);

			lock (_SyncObj)
			{
				_Buffer.AppendLine(DateTime.Now.ToString());
				_Buffer.AppendLine("------------------------------------------");
				_Buffer.AppendLine("LogType: " + logType);
				_Buffer.AppendFormat(msg, args);
				_Buffer.AppendLine();
				_Buffer.AppendLine();

				if (_Buffer.Length > BufferLength) Flush();
			}
		}

		private void Flush()
		{
			lock (_SyncObj)
			{
				if (_Buffer.Length > 0)
				{
					string filePath = GetLogFilePath(this.LogName);
					using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
					{
						sw.Write(_Buffer.ToString());
					}

					_Buffer.Length = 0;
				}
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
