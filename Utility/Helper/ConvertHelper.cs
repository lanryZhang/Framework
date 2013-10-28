using System;

namespace Ifeng.Utility.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ConvertHelper
    {
        public static int ToInt32(object value, int defaultValue)
        {
            try
            {
                return Int32.Parse(value.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime ToDateTime(string str)
        {
            return ToDateTime(str, DateTime.MinValue);
        }

        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            DateTime result;
            if (obj == null)
            {
                return defaultValue;
            }
            if (!DateTime.TryParse(obj.ToString(), out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static DateTime ToDateTime(string str, DateTime defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParse(str, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static DateTime? ToDateTimeNull(object obj, DateTime? defaultValue)
        {
            DateTime result;
            if (obj == null)
            {
                return defaultValue;
            }
            if (!DateTime.TryParse(obj.ToString(), out result))
            {
                return defaultValue;
            }
            return new DateTime?(result);
        }

        public static T ToEnum<T>(string str)
        {
            return ToEnum<T>(str, default(T), true);
        }

        public static T ToEnum<T>(object obj, T defaultValue)
        {
            if (obj != null)
            {
                return ToEnum<T>(obj.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static T ToEnum<T>(string str, T defaultValue)
        {
            return ToEnum<T>(str, defaultValue, true);
        }

        public static T ToEnum<T>(string str, T defaultValue, bool ignoreCase)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), str, ignoreCase);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
