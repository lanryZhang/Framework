using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Ifeng.Utility.Extension
{
    public static class Extensions
    {
        public static TValue SafeGet<TKey,TValue>(this Dictionary<TKey,TValue> dict,TKey key)
        {
            TValue value;
            dict.TryGetValue(key, out value);
            return value;
        }

        public static object SafeGet(this Dictionary<string, object> dict, string key)
        {
            object value;
            dict.TryGetValue(key, out value);
            return value;
        }

        public static string ToMillisecondString(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", new object[] { ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds });
        }

        public static string GetDescription(this Enum value)
        {
            var t = value.GetType();

            string name = Enum.GetName(t, value);
            if (name != null)
            {
                FieldInfo fi = t.GetField(name);
                if (fi != null)
                {
                    var attr = Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute), false) as DescriptionAttribute; ;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
                else
                {
                    return name;
                }
            }
            return string.Empty;
        }
    }
}
