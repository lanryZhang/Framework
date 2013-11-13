using System;
using System.Collections.Generic;

using System.Text;
using Ifeng.DataAccess.NoSql.MongoDb.Provider;
using Ifeng.DataAccess.Sql;
using Ifeng.DataAccess.Sql.Provider;
using Ifeng.Utility.ConfigManager;
using Ifeng.DataAccess.NoSql.Redis;
using Ifeng.Utility.Helper;
using ServiceStack.Redis;

namespace Ifeng.DataAccess
{
    public class DbFactory
    {
        private static DbFactory _instance;

        public static DbFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Instance)
                    {
                        if (_instance == null)
                            _instance = new DbFactory();
                        
                    }
                }
                return _instance;
            }
        }

        public static SqlProvider CreateDbProvider(string dbName)
        {
            var connStr = ConfigManager.Configs["DataBase"][dbName]["value"];
            var provider = ConfigManager.Configs["dbProvider"]["sqlServer"]["value"];
            
            return (SqlProvider)Activator.CreateInstance(Type.GetType(provider), new string[] { connStr });
        }

        public static MongoDbProvider CreateMongoDbProvider(string key)
        {
            var dbInfo = new MongoDbInfo(key);
            var provider = new MongoDbProvider(dbInfo.ConName, dbInfo.DbName);
            return provider;
        }

        public static RedisProvider CreateRedisProvider(string key)
        {
            var ip = ConfigManager.Configs["redis"]["redis1"]["ip"];
            var port = 0;
            Int32.TryParse(ConfigManager.Configs["redis"]["redis1"]["port"], out port);
            if (port == 0)
            {
                throw new NullReferenceException("端口需要为整数");
            }
            var pwd = ConfigManager.Configs["redis"]["redis1"]["pwd"];

            if (string.IsNullOrEmpty(pwd))
            {
                return new RedisProvider(ip, port);
            }
            else
            {
                return new RedisProvider(ip, port, pwd);
            }
        }
    }
}
