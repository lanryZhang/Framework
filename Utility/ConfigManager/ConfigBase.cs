using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Ifeng.Utility.ConfigManager
{
    public class ConfigBase
    {
        public static string GetConfigFilePath(string configFileName, bool checkExists)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\" + configFileName);
            if (checkExists && !File.Exists(filePath)) throw new IOException(string.Format("文件{0}不存在", filePath));

            return filePath;
        }

        public static string[] FindConfigFiles(string patternName)
        {
            string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config");
            string[] filePaths = Directory.GetFiles(dirPath, patternName);
            return filePaths;
        }

        public static XmlDocument GetConfigXml(string configFileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetConfigFilePath(configFileName, true));
            return doc;
        }
    }
}
