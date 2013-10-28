using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Ifeng.Utility.ConfigManager
{
    public class ConfigProvider : ConfigBase
    {
        private Dictionary<string, Dictionary<string, string>> _set = new Dictionary<string, Dictionary<string, string>>();

        public ConfigProvider(string path,  string tagName)
        {
            var doc = GetConfigXml(path);

            var el = doc.GetElementsByTagName(tagName);

            for (var i = 0; i < el.Count; i++)
            {
                var attrs = el[i].Attributes;
                _set.Add(attrs["name"].Value, new Dictionary<string, string>());
                for (var j = 0; j < attrs.Count;j++ )
                {
                    if (!_set.ContainsKey(attrs[j].Value))
                    {
                        _set[attrs["name"].Value].Add(attrs[j].Name, attrs[j].Value);
                    }
                }
            }
        }

        public string GetValue(string name)
        {
            return _set.ContainsKey(name) ? _set[name]["value"] : string.Empty;
        }

        public Dictionary<string,string> this[string name]
        {
            get
            {
                return _set.ContainsKey(name) ? _set[name] : new Dictionary<string,string>();
            }

        }

        public string this[string name,string value]
        {
            get
            {
                return _set.ContainsKey(name) ? _set[name][value] : string.Empty;
            }
            
        }
    }
}
