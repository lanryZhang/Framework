using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Ifeng.DataAccess.Common;
using Ifeng.Utility.Collection;
using Ifeng.Utility.Helper;
using Ifeng.Utility.ORM;
using Ifeng.Utility.Data;

namespace Ifeng.DataAccess.Sql.Provider
{
    /// <summary>
    /// SqlServer
    /// </summary>
    internal class SqlServerProvider : SqlProvider
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connName"></param>
        public SqlServerProvider(string connName)
            : base(connName)
        {
        }

        #endregion

        #region Select
        /// <summary>
        /// 执行一条Sql查询语句，无返回值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <param name="action"></param>
        public override void Select(string table, Select select, Action<IDataReader> action)
        {
            SqlCommand cmd = CreateSelectCommand(table, select);
            Execute(cmd, action);
        }

        /// <summary>
        /// 执行一条Sql查询语句，返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public override T Select<T>(string table, Select select)
        {
            SqlCommand cmd = CreateSelectCommand(table, select);
            return Execute<T>(cmd);
        }

        /// <summary>
        /// 执行一条Sql查询语句，返回一个实体集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override void SelectList(string table, Select select,
            Action<IDataReader> action)
        {
            SqlCommand cmd = CreateSelectCommand(table, select);
            Execute(cmd, action);
        }

        /// <summary>
        /// 查询数据集，返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public override DataList<T> SelectList<T>(string table, Select select)
        {
            SqlCommand cmd = CreateSelectCommand(table, select);
            return ExecuteList<T>(cmd);
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
        public override int Delete(string table, Where where, bool beginTranscation = false)
        {
            SqlCommand cmd = CreateDeleteCommand(table, where, beginTranscation);
            return ExecuteNonQuery(cmd, beginTranscation);
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
        public override int Insert(string table, IDictionary<string, object> fields, bool returnID)
        {
            SqlCommand cmd = CreateInsertCommand(table, fields, returnID);
            if (returnID)
            {
                ExecuteNonQuery(cmd);
                return ConvertHelper.ToInt32(cmd.Parameters["@returnID"].Value, 0);
            }
            return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 批量插入数据(此方法不支持事务)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public override void Insert(string tableName, DataTable dt)
        {
            //using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, _Transaction))
            SqlTransaction tran = null;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, tran))
            {
                try
                {
                     conn.Open();

                    sbc.DestinationTableName = tableName;
                    sbc.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("BulkInsert [{0}]", tableName), ex);
                }
                finally
                {
                    if (tran == null) conn.Dispose();

                }
            }
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
        public override int Update(string table, IDictionary<string, object> fields, Where where)
        {
            SqlCommand cmd = CreateUpdateCommand(table, fields, where);
            return ExecuteNonQuery(cmd);
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
        public override DataList<T> ExecuteList<T>(string procedureName, SqlParam param)
        {
            SqlCommand cmd = CreateProcedureCommand(procedureName, param);
            return ExecuteList<T>(cmd);
        }

        /// <summary>
        /// 执行存储过程，根据传入的Action处理返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public override void ExecuteList(string procedureName, SqlParam param,
            Action<IDataReader> action)
        {
            SqlCommand cmd = CreateProcedureCommand(procedureName, param);
            Execute(cmd, action);
        }

        /// <summary>
        /// 执行存储过程，返回一条结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override T Execute<T>(string procedureName, SqlParam param)
        {
            SqlCommand cmd = CreateProcedureCommand(procedureName, param);
            return Execute<T>(cmd);
        }

        /// <summary>
        /// 执行存储过程，返回结果
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override int Execute(string procedureName, SqlParam param)
        {
            SqlCommand cmd = CreateProcedureCommand(procedureName, param);
            return ExecuteNonQuery(cmd);
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
        public override int ExecuteSql(string sql, SqlParam param, bool beginTranscation = false)
        {
            SqlCommand cmd = CreateCommand(CommandType.Text, beginTranscation);
            cmd.CommandText = sql;

            foreach (SqlParameterItem item in param.ParameterList)
            {
                if (item.Direction == ParameterDirection.Input)
                {
                    cmd.Parameters.AddWithValue(item.Name, item.Value);
                }
                else
                {
                    cmd.Parameters.Add(item.Name, item.DbType);
                }
            }
            return ExecuteNonQuery(cmd, beginTranscation);
        }

        /// <summary>
        /// 执行指定的Sql并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public override void ExecuteSql(string sql, SqlParam param, Action<IDataReader> action)
        {
            SqlCommand cmd = CreateCommand(CommandType.Text);
            cmd.CommandText = sql;
            foreach (SqlParameterItem item in param.ParameterList)
            {
                if (item.Direction == ParameterDirection.Input)
                {
                    cmd.Parameters.AddWithValue(item.Name, item.Value);
                }
                else
                {
                    cmd.Parameters.Add(item.Name, item.DbType);
                }
            }
            Execute(cmd, action);
        }
        #endregion
    }
}