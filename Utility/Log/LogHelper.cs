
using System;
using Ifeng.Utility.Ioc;
using Ifeng.Utility.Helper;

namespace Ifeng.Utility.Log
{
    public static class LogHelper
    {
        private static ILogWriter _LogWriter = IocContainer.Default.GetInstance<ILogWriter>() ?? new TextLogWriter();

        #region Safe

        public static void SafeWriteMessage(string logName, string message, params object[] args)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () => 
            {
                _LogWriter.WriteMessage(logName, message, args);
            });            
        }

        public static void SafeWriteWarning(string logName, string message, params object[] args)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () =>
            {
                _LogWriter.WriteWarning(logName, message, args);
            });            
        }

        public static void SafeWriteError(string logName, string message, params object[] args)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () =>
            {
                _LogWriter.WriteError(logName, message, args);
            });
        }

        public static void SafeWriteException(Exception ex)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () =>
            {
                WriteException(ex.GetType().Name, ex);
            });
        }

        public static void SafeWriteException(string logName, Exception ex)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () =>
            {
                _LogWriter.WriteException(logName, ex);
            });
        }

        public static void SafeWriteLog(LogType logType, string logName, string message, params object[] args)
        {
            RetryHelper.Retry(3, TimeSpan.FromMilliseconds(500), false, () =>
            {
                _LogWriter.WriteLog(logType, logName, message, args);
            });
        }

        #endregion

        #region Unsafe

        public static void WriteMessage(string logName, string message, params object[] args)
        {
            _LogWriter.WriteMessage(logName, message, args);
        }

        public static void WriteWarning(string logName, string message, params object[] args)
        {
            _LogWriter.WriteWarning(logName, message, args);
        }

        public static void WriteError(string logName, string message, params object[] args)
        {
            _LogWriter.WriteError(logName, message, args);
        }

        public static void WriteException(Exception ex)
        {
            WriteException(ex.GetType().Name, ex);
        }

        public static void WriteException(string logName, Exception ex)
        {
            _LogWriter.WriteException(logName, ex);
        }

        public static void WriteLog(LogType logType, string logName, string message, params object[] args)
        {
            _LogWriter.WriteLog(logType, logName, message, args);
        }

        #endregion

    }

}
