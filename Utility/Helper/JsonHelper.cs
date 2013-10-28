using System;
using System.Collections.Generic;
using System.Text;
using Ifeng.Json;
using Newtonsoft.Json;
using Ifeng.Json;

namespace Ifeng.Utility.Helper
{
    public static class JsonHelper
    {
        // Fields
        private static readonly JsonConverter _Converter = new IJsonConverter();

        // Methods
        public static T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static object Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }

        public static object Deserialize(string jsonString, Type type)
        {
            return JsonConvert.DeserializeObject(jsonString, type);
        }

        public static string Serialize(IJson obj)
        {
            return obj.ToJsonString();
        }

        public static string Serialize(object obj)
        {
            IJson jsonObj = obj as IJson;
            if (jsonObj != null)
            {
                return Serialize(jsonObj);
            }
            return JsonConvert.SerializeObject(obj, new JsonConverter[] { _Converter });
        }
    }


}
