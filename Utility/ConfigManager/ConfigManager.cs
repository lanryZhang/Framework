using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Ifeng.Utility.ConfigManager
{
    public class ConfigManager
    {
        private static Dictionary<string, ConfigProvider> dict =
            new Dictionary<string, ConfigProvider>() {
                    {"DataBase",new ConfigProvider("DataBase.config", "db")},
                    {"dbProvider",new ConfigProvider("DbProvider.config", "dbinfo")},
                    {"myqueue",new ConfigProvider("Messaging.config", "msg")},
                    {"redis",new ConfigProvider("Redis.config", "tcpInfo")},
                    {"MongoDB",new ConfigProvider("DataBase.config", "db")},
                };

        public static Dictionary<string, ConfigProvider> Configs
        {
            get
            {
                return dict;
            }
        }
    }
}
