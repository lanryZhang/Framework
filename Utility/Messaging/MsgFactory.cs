using System;
using System.Collections.Generic;
using System.Text;
using Ifeng.Utility.ConfigManager;

namespace Ifeng.Utility.Messaging
{
    public class MsgFactory
    {
        public static MsgService CraeteMsgService(string pathKey)
        {
            var path = ConfigManager.ConfigManager.Configs["myqueue"][pathKey, "value"];
            return new MsgService(path);
        }
    }
}
