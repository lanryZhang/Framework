using System;
using System.IO;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Ifeng.Utility.Helper
{
    public class XmlHelper
    {
		public static string XmlSerialize(object o, XmlSerializerNamespaces namespaces)
		{
			return XmlSerialize(o, o.GetType(), namespaces);
		}
		public static string XmlSerialize(object o, System.Text.Encoding encoding, XmlSerializerNamespaces namespaces)
		{
			return XmlSerialize(o, o.GetType(), encoding, namespaces);
		}
		public static string XmlSerialize(object o, Type type, XmlSerializerNamespaces namespaces)
		{
			return XmlSerialize(o, type, System.Text.Encoding.GetEncoding("utf-8"), namespaces);
		}
		public static string XmlSerialize(object o, Type type, System.Text.Encoding encoding, XmlSerializerNamespaces namespaces)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(type);
			using(MemoryStream memoryStream = new MemoryStream())
			{
				XmlTextWriter xmlWriter = new XmlTextWriter(memoryStream,encoding);
				xmlSerializer.Serialize(xmlWriter, o, namespaces);

				string result = encoding.GetString(memoryStream.ToArray()).Trim();

				xmlWriter.Close();

				memoryStream.Close();

				return result;
			}
		}

        public static string XmlSerialize(object o)
        {
            return XmlSerialize(o, o.GetType());
        }
        public static string XmlSerialize(object o, System.Text.Encoding encoding)
        {
            return XmlSerialize(o, o.GetType(), encoding);
        }
        public static string XmlSerialize(object o, Type type)
        {
            return XmlSerialize(o, type, System.Text.Encoding.GetEncoding("utf-8"));
        }
        public static string XmlSerialize(object o, Type type, System.Text.Encoding encoding)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(memoryStream, encoding);
                xmlSerializer.Serialize(xmlWriter, o);

                string result = encoding.GetString(memoryStream.ToArray()).Trim();

                xmlWriter.Close();

                memoryStream.Close();

                return result;
            }
        }

		public static object XmlDeserialize(string xml, Type type)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(type);
			System.Text.Encoding encoding = null;
			try
			{
				string coding = (new Regex("<\\?xml[^\\?]*encoding=\"(?<encoding>[^\"]*)\"[^\\?]*\\?>")).Match(xml).Result("${encoding}");
				encoding = System.Text.Encoding.GetEncoding(coding);
			}
			catch
			{
			}
			if(encoding == null)
			{
				encoding = System.Text.ASCIIEncoding.ASCII;
			}

			using(MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(xml)))
			{
				object obj = xmlSerializer.Deserialize(memoryStream);

				memoryStream.Close();

				return obj;
			}
		}
        /// <summary>
        /// 文本化XML序列化
        /// </summary>
        /// <param name="item">对象</param>
        public static string ToXml<T>(T item)
        {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, item);
                return sb.ToString();
            }
        }

        /// <summary>
        /// 文本化XML反序列化
        /// </summary>
        /// <param name="str">字符串序列</param>
        public static T FromXml<T>(string str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (XmlReader reader = new XmlTextReader(new StringReader(str)))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
