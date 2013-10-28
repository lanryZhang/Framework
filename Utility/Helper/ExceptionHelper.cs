using System;
using System.Reflection;
using System.Web;
using System.Web.Services.Protocols;

namespace Ifeng.Utility.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemSupportException : ApplicationException
    {
        /// <summary>
        /// Methods
        /// </summary>
        public SystemSupportException()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMessage"></param>
        public SystemSupportException(string strMessage)
            : base(strMessage)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="ex"></param>
        public SystemSupportException(string strMessage, Exception ex)
            : base(strMessage, ex)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Methods
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramName"></param>
        public static void CheckStringIsNullOrEmpty(string data, string paramName)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("字符串为空！");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parseExpressionResult"></param>
        /// <param name="message"></param>
        /// <param name="messageParams"></param>
        public static void FalseThrow(bool parseExpressionResult, string message, params object[] messageParams)
        {
            TrueThrow(!parseExpressionResult, message, messageParams);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseExpressionResult"></param>
        /// <param name="message"></param>
        /// <param name="messageParams"></param>
        public static void FalseThrow<T>(bool parseExpressionResult, string message, params object[] messageParams) where T : Exception
        {
            TrueThrow<T>(!parseExpressionResult, message, messageParams);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception GetRealException(Exception ex)
        {
            Exception lastestEx = ex;
            while ((ex != null) && (((ex is HttpUnhandledException) || (ex is HttpException)) || (ex is TargetInvocationException)))
            {
                if (ex.InnerException != null)
                {
                    lastestEx = ex.InnerException;
                }
                else
                {
                    lastestEx = ex;
                }
                ex = ex.InnerException;
            }
            return lastestEx;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetSoapExceptionMessage(Exception ex)
        {
            string strNewMsg = ex.Message;
            if (ex is SoapException)
            {
                int i = strNewMsg.LastIndexOf("--> ");
                if (i <= 0)
                {
                    return strNewMsg;
                }
                strNewMsg = strNewMsg.Substring(i + 4);
                i = strNewMsg.IndexOf(": ");
                if (i > 0)
                {
                    strNewMsg = strNewMsg.Substring(i + 2);
                    i = strNewMsg.IndexOf("\n   ");
                    strNewMsg = strNewMsg.Substring(0, i);
                }
            }
            return strNewMsg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parseExpressionResult"></param>
        /// <param name="message"></param>
        /// <param name="messageParams"></param>
        public static void TrueThrow(bool parseExpressionResult, string message, params object[] messageParams)
        {
            TrueThrow<SystemSupportException>(parseExpressionResult, message, messageParams);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseExpressionResult"></param>
        /// <param name="message"></param>
        /// <param name="messageParams"></param>
        public static void TrueThrow<T>(bool parseExpressionResult, string message, params object[] messageParams) where T : Exception
        {
            Type exceptionType = typeof(T);
            if (parseExpressionResult)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }
                object obj = Activator.CreateInstance(exceptionType);
                Type[] types = new Type[] { typeof(string) };
                ConstructorInfo constructorInfoObj = exceptionType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.HasThis, types, null);
                object[] args = new object[] { string.Format(message, messageParams) };
                constructorInfoObj.Invoke(obj, args);
                throw ((Exception)obj);
            }
        }
    }
}
