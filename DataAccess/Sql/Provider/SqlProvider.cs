using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Ifeng.DataAccess.Common;
using Ifeng.Utility.Collection;
using Ifeng.Utility.ORM;
using Ifeng.Utility.Data;


namespace Ifeng.DataAccess.Sql.Provider
{
    public abstract class SqlProvider
    {
        protected string ConnectionString { set; get; }

        protected SqlProvider(string connName)
        {
            ConnectionString = connName;
        }

        #region Select
        public virtual void Select(string table,
            Select select, Action<IDataReader> action)
        {

        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public virtual T Select<T>(string table, Select select) where T : IDataRow, new()
        {
            return default(T);
        }

        /// <summary>
        /// 执行一条Sql查询语句，返回一个实体集
        /// </summary>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual void SelectList(string table,
            Select select, Action<IDataReader> action)
        {
        }

        /// <summary>
        /// 查询数据集，返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public virtual DataList<T> SelectList<T>(string table, Select select)
            where T : IDataRow, new()
        {
            return default(DataList<T>);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        public virtual int Delete(string table, 
            Where where, bool beginTranscation = false)
        {
            return 0;
        }
        #endregion

        #region Insert
        /// <summary>
        /// Insert 手动输入需要插入的字段值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="returnID"></param>
        /// <returns></returns>
        public virtual int Insert(string table, 
            IDictionary<string, object> fields, bool returnID)
        {
            return 0;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public virtual void Insert(string tableName, DataTable dt)
        {
        }
        #endregion

        #region Update
        /// <summary>
        /// Update：手动输入需要更新的字段，执行更新语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual int Update(string table, IDictionary<string, object> fields, Where where)
        {
            return 0;
        }

        #endregion

        #region Execute Procedure
        /// <summary>
        /// 执行存储过程，返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual DataList<T> ExecuteList<T>(string procedureName, SqlParam param)
            where T : IDataRow, new()
        {
            return default(DataList<T>);
        }

        /// <summary>
        /// 执行存储过程，返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <param name="actoin"></param>
        /// <returns></returns>
        public virtual void ExecuteList(string procedureName,
            SqlParam param, Action<IDataReader> actoin)
        {
        }

        /// <summary>
        /// 执行存储过程，返回一条结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual T Execute<T>(string procedureName, SqlParam param)
            where T : IDataRow, new()
        {
            return default(T);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual int Execute(string procedureName, SqlParam param)
        {
            return 0;
        }
        #endregion

        #region Execute Sql

        /// <summary>
        /// 执行指定的Sql语句 Insert/Update
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        public virtual int ExecuteSql(string sql, SqlParam param, bool beginTranscation = false)
        {
            return 0;
        }

        /// <summary>
        /// 执行指定的Sql并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public virtual void ExecuteSql(string sql, SqlParam param, Action<IDataReader> action)
        {
        }
        #endregion

        #region protected Execute
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="action"></param>
        protected void Execute(SqlCommand cmd, Action<IDataReader> action)
        {
            using (cmd)
            {
                try
                {
                    cmd.Connection.Open();

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        action(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected T Execute<T>(SqlCommand cmd) 
            where T : IDataRow, new()
        {
            var entity = new T();
            using (cmd)
            {
                try
                {
                    cmd.Connection.Open();

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var loader = DataLoader.Create(reader);
                            loader.LoadData(entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.Connection.Dispose();
                }
            }
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(SqlCommand cmd, bool beginTranscation)
        {
            using (cmd)
            {
                try
                {
                    cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    if (beginTranscation)
                        cmd.Transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new Exception(ex.Message);

                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.Connection.Dispose();
                    if (beginTranscation)
                    {
                        cmd.Transaction.Dispose();
                        cmd.Transaction = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(SqlCommand cmd)
        {
            using (cmd)
            {
                try
                {
                    cmd.Connection.Open();

                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected DataList<T> ExecuteList<T>(SqlCommand cmd)
            where T : IDataRow, new()
        {
            DataList<T> list = new DataList<T>();
            
            using (cmd)
            {
                try
                {
                    cmd.Connection.Open();

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T entity = new T();
                            var loader = DataLoader.Create(reader);
                            loader.LoadData(entity);
                            list.Add(entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.Connection.Dispose();
                }
            }
            return list;
        }

        #endregion

        #region CreateCommand
        /// <summary>
        /// 创建插入命令
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldsValues"></param>
        /// <param name="returnID"></param>
        /// <returns></returns>
        protected SqlCommand CreateInsertCommand(string table, IDictionary<string, object> fieldsValues, bool returnID = false)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            SqlCommand cmd = CreateCommand(CommandType.Text);

            int i = 0;
            string[] fields = new string[fieldsValues.Count];
            string[] values = new string[fieldsValues.Count];

            foreach (var kvp in fieldsValues)
            {
                fields[i] = "[" + kvp.Key + "]";
                values[i++] = "@" + kvp.Key;
                if (kvp.Value != null)
                {
                    cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@" + kvp.Key, DBNull.Value);
                }
            }
            if (returnID)
            {
                SqlParameter param = new SqlParameter("@returnID", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);
            }

            sqlBuilder.AppendFormat("INSERT INTO {0}({1}) VALUES({2})", table.Replace(" with(nolock)", ""), string.Join(",", fields), string.Join(",", values));
            if (returnID) sqlBuilder.Append(" set @returnID=SCOPE_IDENTITY();");
            cmd.CommandText = sqlBuilder.ToString();

            return cmd;
        }

        /// <summary>
        /// 创建删除命令
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        protected SqlCommand CreateDeleteCommand(string table, Where where, bool beginTranscation = false)
        {
            var sqlBuilder = new StringBuilder();
            SqlCommand cmd = CreateCommand(CommandType.Text, beginTranscation);

            sqlBuilder.Append("DELETE FROM ").Append(table.Replace(" with(nolock)", ""));

            if (where != null && where.List.Count != 0)
            {
                sqlBuilder.Append(" WHERE ");
                foreach (WhereItem item in where.List)
                {
                    sqlBuilder.Append(BuildWhereCondition(item)).Append(" AND ");

                    if (item.Value != null)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Name, item.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@" + item.Name, DBNull.Value);
                    }
                }

                sqlBuilder.Remove(sqlBuilder.Length - 4, 4);
            }
            cmd.CommandText = sqlBuilder.ToString();
            return cmd;
        }

        /// <summary>
        /// 创建更新命令
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldValues"></param>
        /// <param name="where"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        protected SqlCommand CreateUpdateCommand(string table, IDictionary<string, object> fieldValues, Where where, bool beginTranscation = false)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            SqlCommand cmd = CreateCommand(CommandType.Text, beginTranscation);
            StringBuilder fieldsBuilder = new StringBuilder();

            foreach (var item in fieldValues)
            {
                fieldsBuilder.Append(item.Key).Append("=@").Append(item.Key).Append(", ");

                if (item.Value != null)
                {
                    cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@" + item.Key, DBNull.Value);
                }
            }
            fieldsBuilder.Remove(fieldsBuilder.Length - 2, 2);

            sqlBuilder.Append("UPDATE ").Append(table.Replace(" with(nolock)", "")).Append(" SET ").Append(fieldsBuilder);

            if (where != null && where.List.Count != 0)
            {
                sqlBuilder.Append(" WHERE ");
                foreach (WhereItem item in where.List)
                {
                    sqlBuilder.Append(BuildWhereCondition(item)).Append(" AND ");
                    cmd.Parameters.AddWithValue("@" + item.Name, item.Value);
                }
                sqlBuilder.Remove(sqlBuilder.Length - 4, 4);
            }
            cmd.CommandText = sqlBuilder.ToString();
            return cmd;
        }

        /// <summary>
        /// 创建查找命令
        /// </summary>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        protected SqlCommand CreateSelectCommand(string table, Select select)
        {
            SqlCommand cmd = CreateCommand(CommandType.Text);
            StringBuilder sqlBuilder = new StringBuilder();
            var where = new StringBuilder();

            if (select.Condition != null && select.Condition.List.Count != 0)
            {
                where.Append(" WHERE ");
                foreach (WhereItem item in select.Condition.List)
                {
                    where.Append(BuildWhereCondition(item)).Append(" AND ");

                    if (item.Value != null)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Name, item.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@" + item.Name, DBNull.Value);
                    }
                }
                where.Remove(where.Length - 4, 4);
            }

            if (!string.IsNullOrEmpty(select.OverOrderByString))
            {
                sqlBuilder.AppendFormat("with t as (SELECT row_number() over(order by {0}) as row_index,*", select.OverOrderByString)
                    .Append(" FROM ").Append(table)
                    .Append(where)
                    .AppendFormat(") select {0} from t", string.Join(",", select.Fields));
            }
            else
            {
                sqlBuilder.AppendFormat("SELECT {0}", string.Join(",", select.Fields))
                    .Append(" FROM ").Append(table)
                    .Append(where);
            }

            cmd.CommandText = sqlBuilder.ToString();
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="beginTranscation"></param>
        /// <returns></returns>
        protected SqlCommand CreateCommand(CommandType type, bool beginTranscation = false)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = type;
            cmd.CommandTimeout = 60;

            if (beginTranscation)
            {
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.Transaction = cmd.Connection.BeginTransaction();
            }
            else
            {
                cmd.Connection = new SqlConnection(ConnectionString);
            }

            return cmd;
        }

        /// <summary>
        /// 创建存储过程执行命令
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected SqlCommand CreateProcedureCommand(string procedureName, SqlParam param)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 60;

            cmd.Connection = new SqlConnection(ConnectionString);
            cmd.CommandText = procedureName;
            foreach (SqlParameterItem item in param.ParameterList)
            {
                if (item.Direction == ParameterDirection.Output)
                {
                    var p = new SqlParameter(item.Name, item.DbType, item.Size);
                    p.Direction = item.Direction;
                    cmd.Parameters.Add(p);
                    param.SetOutputParam(item.Name, p);
                }
                else
                {
                    var p = new SqlParameter(item.Name, item.Value ?? DBNull.Value);
                    p.Direction = item.Direction;
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }
        #endregion

        #region BuidlCondition
        /// <summary>
        /// 创建条件语句
        /// </summary>
        /// <param name="whereItem"></param>
        /// <returns></returns>
        internal string BuildWhereCondition(WhereItem whereItem)
        {
            StringBuilder whereBuild = new StringBuilder();

            switch (whereItem.WhereType)
            {
                case WhereType.Equal:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, "=@");
                    break;
                case WhereType.ExplicitLike:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, " like @");
                    break;
                case WhereType.GreaterAndEqual:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, ">=@");
                    break;
                case WhereType.GreaterThan:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, ">@");
                    break;
                case WhereType.In:
                    whereBuild.AppendFormat("{0} in ({1})", whereItem.Name, whereItem.Value);
                    break;
                case WhereType.RightLike:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, " like '%'+@");
                    break;
                case WhereType.LessAndEqual:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, "<=@");
                    break;
                case WhereType.LessThan:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, "<@");
                    break;
                case WhereType.Like:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, " like '%'+ @").Append(" +'%'");
                    break;
                case WhereType.NotEqual:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, " <> @");
                    break;
                case WhereType.LeftLike:
                    whereBuild.AppendFormat("{0}{1}{0}", whereItem.Name, " like @").Append("+'%'");
                    break;
            }

            return whereBuild.ToString();
        }
        #endregion
    }


}
