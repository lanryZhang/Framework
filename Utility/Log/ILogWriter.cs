
using System;
using System.Collections.Generic;
using System.Text;

namespace Ifeng.Utility.Log
{
    public interface ILogWriter
    {
		void WriteMessage(string logName, string message, params object[] args);
		void WriteWarning(string logName, string message, params object[] args);
		void WriteError(string logName, string message, params object[] args);
		void WriteException(string logName, Exception ex);
		void WriteLog(LogType logType, string logName, string message, params object[] args);
	}

}
