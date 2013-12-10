using System;
using System.Collections.Generic;

using System.Text;
using Ifeng.DataAccess.Sql.Provider;
using System.Data;
using MongoDB;
using Ifeng.Utility.ConfigManager;

namespace Ifeng.DataAccess.NoSql.MongoDb.Provider
{
    public class MongoDbInfo
    {
        private static string _dbName = string.Empty;
        private static string _conName = string.Empty;

        public string DbName
        {
            get { return _dbName; }
        }

        public string ConName
        {
            get { return _conName; }
        }

        public MongoDbInfo(string key)
        {
            var dbInfo = ConfigManager.Configs["Mongo"][key]["value"];
            if (dbInfo != null)
            {
                var arr = dbInfo.Split(',');
                if (arr.Length < 2)
                {
                    throw new Exception("数据库连接字符串配置格式不正确.");
                }

                _dbName = dbInfo.Split(',')[1];
                _conName = dbInfo.Split(',')[0];
            }
            else
            {
                throw new Exception("无法找到对应数据库配置.");
            }
        }
    }
}
