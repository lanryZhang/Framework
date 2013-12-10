using Ifeng.DataAccess.NoSql.MongoDb.Provider;
using Ifeng.DataAccess.Sql.Provider;
using Ifeng.DataAccess.NoSql.Redis;


namespace Ifeng.DataAccess
{
    /*
     * 
     */
    public class DbProvider
    {
        public static SqlProvider sqlProvider = DbFactory.CreateDbProvider("sqlProvider");

        //public static MongoDbProvider MongoMtime= DbFactory.CreateMongoDbProvider("mongoDb1");

        public static MongoDbProvider GetMongoDb(string dbName)
        {
            return DbFactory.CreateMongoDbProvider(dbName);
        }

        /// <summary>
        /// Create redis client
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static RedisProvider GetRedisInstance(string key)
        {
            return DbFactory.CreateRedisProvider(key);
        }

        public static SqlProvider GetSqlProvider(string dbName)
        {
            return DbFactory.CreateDbProvider(dbName);
        }
    }
}
