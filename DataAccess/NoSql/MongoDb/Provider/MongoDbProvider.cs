using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ifeng.DataAccess.Common;
using Ifeng.Utility.Collection;
using Ifeng.DataAccess.Sql.Provider;
using System.Data;
using MongoDB;
using Ifeng.Utility.ORM;

namespace Ifeng.DataAccess.NoSql.MongoDb.Provider
{
    public class MongoDbProvider : IDisposable
    {
        private readonly Mongo _server;
        private MongoDatabase _database;

        public MongoDbProvider()
            : this("mongodb://127.0.0.1:27017","mtime")
        {
            
        }

        public MongoDbProvider(string conName, string dbName)
        {
            _server = new Mongo(conName);
            _server.Connect();
            if (!string.IsNullOrEmpty(dbName))
            {
                _database = _server.GetDatabase(dbName) as MongoDatabase;    
            }
        }

        public void ChangeDb(string dbName)
        {
            _database = _server.GetDatabase(dbName) as MongoDatabase;
        }

        public MongoDatabase CurrentDb
        {
            get
            {
                if (_database == null)
                {
                    throw new Exception("当前连接没有指定任何数据库。请在构造函数中指定数据库名或者调用ChangeDb()方法切换数据库");
                }
                return _database;
            }
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        private Document CreateWhereQuery(Where where)
        {
            var query = new Document();
            if (where.List != null && where.List.Count > 0)
            {
                foreach (var whereItem in where.List)
                {
                    switch (whereItem.WhereType)
                    {
                        case WhereType.Equal:
                            query.Add(whereItem.Name, whereItem.Value);
                            break;
                        case WhereType.GreaterAndEqual:
                            query.Add(whereItem.Name, string.Format("{{$gte:{0}}}" ,whereItem.Value.ToString()));
                            break;
                        case WhereType.GreaterThan:
                            query.Add(whereItem.Name, string.Format("{{$gt:{0}}}" , whereItem.Value.ToString()));
                            break;
                        case WhereType.In:
                            query.Add(whereItem.Name, string.Format("{{$in:[{0}]}}" ,string.Join(",",(string[])whereItem.Value)));
                            break;
                        case WhereType.LessAndEqual:
                            query.Add(whereItem.Name, string.Format("{{$lte:{0}}}" , whereItem.Value.ToString()));
                            break;
                        case WhereType.LessThan:
                            query.Add(whereItem.Name, string.Format("{{$lt:{0}}}" , whereItem.Value.ToString()));
                            break;
                        case WhereType.LeftLike:
                            query.Add(whereItem.Name,string.Format( "/^{0}.*/i" ,whereItem.Value.ToString()));
                            break;
                        case WhereType.Like:
                            query.Add(whereItem.Name,string.Format( "/{0}/i" ,whereItem.Value.ToString()));
                            break;
                        case WhereType.NotEqual:
                            query.Add(whereItem.Name, string.Format("{{$ne:{0}}}" , whereItem.Value.ToString()));
                            break;
                        case WhereType.RightLike:
                            query.Add(whereItem.Name,string.Format( "/*{0}$/i" ,whereItem.Value.ToString()));
                            break;
                    }
                    
                }
            }

            return query;
        }

        /// <summary>
        /// 根据实体 映射出更新字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <returns></returns>
        public Document CreateUpdateDocument<T>(T en)
        {
            var doc = new Document();
            var dic = ORM.ReflectFields(en);

            foreach (var item in doc)
            {
                if (!doc.ContainsKey(item.Key))
                {
                    doc.Add(item.Key, item.Value);
                }
            }

            return doc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        private MapReduce CreateMapReduce<T>(MgCollection table, CollectionSelect select)
        {
            var values = new StringBuilder();
            var hasCountMethod = false;
            var hasSumMethod = false;
            var hasAvgMethod = false;
            var hasMinMethod = false;
            var hasMaxMethod = false;

            #region Get Group Fields
            foreach (var item in select.Fields)
            {
                if (item.Contains("sum("))
                {
                    hasSumMethod = true;
                    values.Append("sum:this.").Append(item.Replace("sum(","").Replace(")","")).Append(",");
                }

                if (item.Contains("count("))
                {
                    hasCountMethod = true;
                    values.Append("count:1").Append(",");
                }
                if (item.Contains("avg("))
                {
                    if (!hasSumMethod)
                        values.AppendFormat("sum:this.{0}",item.Replace("avg(", "").Replace(")", "")).Append(",");
                    if (!hasCountMethod)
                        values.Append("count:1").Append(",");
                    hasAvgMethod = true;
                }
                if (item.Contains("min"))
                {
                    values.AppendFormat("min:Math.min(this.{0})", item.Replace("min(", "").Replace(")", "")).Append(",");
                    hasMinMethod = true;
                }
                if (item.Contains("max"))
                {
                    values.AppendFormat("max:Math.max(this.{0})", item.Replace("max(", "").Replace(")", "")).Append(",");
                    hasMaxMethod = true;
                }
            }
            var finalValues = string.Empty;
            if (values.Length > 0)
            {
                finalValues = values.Remove(values.Length - 1, 1).ToString();
            }

            var groupByKey = new StringBuilder();

            foreach (var item in select.GroupByString.Split(','))
            {
                groupByKey.AppendFormat("{0}:this.{0},", item);
            }

            groupByKey = groupByKey.Remove(groupByKey.Length - 1, 1);
            #endregion

            //Create MapReduce
            MapReduce mp = CurrentDb.GetCollection(table.TableName.ToString()).MapReduce(); //new MapReduce(CurrentDb, string.Empty, typeof(T));
            
            //Map Code
            var mapCode = new StringBuilder();
            mapCode.Append("function(){")
                   .Append("emit({").Append(groupByKey).Append("},{").Append(finalValues).Append("});}");

            var finalizeFunc = new StringBuilder();

            #region Build Reduce Code
            //Reduce Code
            var reduceCode = new StringBuilder();
            var returnObj = new StringBuilder("return {");
            reduceCode.Append("function(key,values){")
                .Append("var sum = 0; var max = 0;var min = 0; var count = 0;")
                .Append("for(var i = 0; i < values.length;i++){");
            if (hasSumMethod || hasAvgMethod)
            {
                reduceCode.Append("sum += values[i].sum;");
                returnObj.Append("Sum:sum,");
            }
            if (hasCountMethod || hasAvgMethod)
            {
                reduceCode.Append("count += values[i].count;");
                returnObj.Append("Count:count,");
            }
            if (hasMinMethod)
            {
                reduceCode.Append("min = values[i].min;");
                returnObj.Append("Min:min,");
            }
            if (hasMaxMethod)
            {
                reduceCode.Append("max = values[i].max;");
                returnObj.Append("Max:max,");
            }
            if (hasAvgMethod)
            {
                finalizeFunc.Append("function (key, reducedValue) {")
                            .Append("reducedValue.Avg = reducedValue.Sum/reducedValue.Count;")
                            .Append("return reducedValue;}");
            }
            returnObj = returnObj.Length > 8 ? returnObj.Remove(returnObj.Length - 1, 1).Append("}") : new StringBuilder("");
            reduceCode.Append("} ").Append(returnObj).Append(";}");

            if (finalizeFunc.Length > 0)
            {
                mp.Map(new Code((mapCode.ToString())))
                  .Reduce(new Code(reduceCode.ToString()))
                  .Finalize(new Code(finalizeFunc.ToString()));    
            }
            else
            {
                mp.Map(new Code((mapCode.ToString())))
              .Reduce(new Code(reduceCode.ToString()));
            }
            #endregion

            return mp;
        }

        /// <summary>
        /// 查询信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public DataList<T> Select<T>(MgCollection table, CollectionSelect select)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (select == null)
            {
                throw new ArgumentNullException("select");
            }

            var query = CreateWhereQuery(select.Condition);
            IEnumerable<Document> documents;
            var list = new DataList<T>();
            bool userMr = select.GroupByString != null;
            if (userMr)
            {
                var mp = CreateMapReduce<T>(table,select);

                mp.Out("result");
                mp.KeepTemp(false);
                mp.Query(query);
                var l = mp.Documents.ToList<Document>();
                var remainRecord = l.Count - (select.PageIndex - 1) * select.PageSize;
                documents = l.GetRange(select.PageIndex - 1, remainRecord < select.PageSize ? remainRecord : select.PageSize);
            }
            else
            {
                var cursor = CurrentDb.GetCollection(table.TableName.ToString()).Find(query).Skip(select.PageIndex).Limit(select.PageSize);
                documents = cursor.Documents;
            }

            foreach (var item in documents)
            {
                if (userMr)
                {
                    list.Add(ORM.MrDocumentToObject<T>(item));
                }
                else
                {
                    list.Add(ORM.DocumentToObject<T>(item));
                }
            }
            return list;

        }

        #region Delete
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        public void Delete(MgCollection table, Where where)
        {
            var query = CreateWhereQuery(where);
            CurrentDb.GetCollection(table.TableName.ToString()).Remove(query);
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新匹配的记录 没有匹配则插入该记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="en"></param>
        /// <param name="where"></param>
        public void UpdateOrAdd<T>(MgCollection table, T en, Where where)
        {
            var query = CreateWhereQuery(where);
            var document = CreateUpdateDocument(en);
            CurrentDb.GetCollection(table.TableName.ToString()).Update(document, query, UpdateFlags.Upsert);
        }

        /// <summary>
        /// 更新所有匹配到的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="en"></param>
        /// <param name="where"></param>
        public void UpdateAll<T>(MgCollection table, T en, Where where)
        {
            var query = CreateWhereQuery(where);
            var document = CreateUpdateDocument(en);
            CurrentDb.GetCollection(table.TableName.ToString()).Update(document, query, UpdateFlags.MultiUpdate);
        }

        /// <summary>
        /// 更新第一条匹配到的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="en"></param>
        /// <param name="where"></param>
        public void UpdateOne<T>(MgCollection table, T en, Where where)
        {
            var query = CreateWhereQuery(where);
            var document = CreateUpdateDocument(en);
            CurrentDb.GetCollection(table.TableName.ToString()).Update(document, query);
        }
        #endregion

        public void Dispose()
        {
            if (_server == null) return;
            _server.Disconnect();
            _server.Dispose();
        }
    }
}
