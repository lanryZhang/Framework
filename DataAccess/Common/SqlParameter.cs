using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlParam
    {
        /// <summary>
        /// 
        /// </summary>
        internal List<KeyValuePair<string, IDataParameter>> _outPutParameter = null;
        /// <summary>
        /// 
        /// </summary>
        internal List<SqlParameterItem> ParameterList = new List<SqlParameterItem>();

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public SqlParam()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SqlParam(string name, object value)
        {
            ParameterList.Add(new SqlParameterItem(name, value));
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        ///// <param name="dataType"></param>
        //public SqlParameter(string name, object value, SqlDataType dataType)
        //{
        //    _parameterList.Add(new SqlParameterItem(name, value, dataType));
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        ///// <param name="dataType"></param>
        ///// <param name="size"></param>
        //public SqlParameter(string name, object value, SqlDataType dataType, int size)
        //{
        //    _parameterList.Add(new SqlParameterItem(name, value, dataType, size));
        //}
        #endregion

        #region Add Parameter
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public SqlParam Add(string name, object value)
        {
            ParameterList.Add(new SqlParameterItem(name, value));
            return this;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="dbType">参数类型</param>
        /// <returns></returns>
        public SqlParam Add(string name, object value, SqlDbType dbType)
        {
            ParameterList.Add(new SqlParameterItem(name, value, dbType));
            return this;
        }


        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <returns></returns>
        public SqlParam AddOuput(string name,SqlDbType dbType)
        {
            ParameterList.Add(new SqlParameterItem(name, ParameterDirection.Output, 0));
            if (_outPutParameter == null)
            {
                _outPutParameter = new List<KeyValuePair<string, IDataParameter>>();
            }
            else
            {
                SqlParameter param = new System.Data.SqlClient.SqlParameter(name, dbType);

                _outPutParameter.Add(new KeyValuePair<string, IDataParameter>(name, param));
            }
            return this;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns></returns>
        public SqlParam AddOuput(string name,SqlDbType dbType, int size)
        {
            ParameterList.Add(new SqlParameterItem(name,dbType, ParameterDirection.Output, size));
            if (_outPutParameter == null)
            {
                _outPutParameter = new List<KeyValuePair<string, IDataParameter>>();
            }
            else
            {
                SqlParameter param = new System.Data.SqlClient.SqlParameter(name,dbType,size);

                _outPutParameter.Add(new KeyValuePair<string, IDataParameter>(name, param));
            }
            return this;
        }
        #endregion

        /// <summary>
        /// 获取输出参数的值
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns></returns>
        public object GetReturnValue(string name)
        {
            if (_outPutParameter != null)
            {
                foreach (KeyValuePair<string, IDataParameter> temp in _outPutParameter)
                {
                    if (temp.Key == name)
                    {
                        return temp.Value.Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取输出参数的值 并指定参数类型
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <returns></returns>
        public T GetReturnValue<T>(string name)
        {
            object value = GetReturnValue(name);
            return value == null ? default(T) : (T)value;
        }

        internal void SetOutputParam(string name, IDataParameter param)
        {
            if (_outPutParameter == null) _outPutParameter = new List<KeyValuePair<string, IDataParameter>>();
            _outPutParameter.Add(new KeyValuePair<string, IDataParameter>(name, param));
        }

        /// <summary>
        /// 重新设置参数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParam ResetParamValue(string name, object value)
        {
            var position = 0;
            for (var i = ParameterList.Count - 1;i >=0;i--)
            {
                if (ParameterList[i].Name.ToLower() == name)
                {
                    position = i;
                    break;
                }
            }

            ParameterList.RemoveAt(position);
            ParameterList.Add(new SqlParameterItem(name, value));
            return this;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class SqlParameterItem
    {
        private string _name;
        private object _value;
        private ParameterDirection _direction;
        private SqlDbType _dbType;
        private int _size;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SqlParameterItem(string name, object value)
        {
            _name = name;
            _value = value;
            _direction = ParameterDirection.Input;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public SqlParameterItem(string name, object value, SqlDbType dbType)
        {
            _name = name;
            _value = value;
            _dbType = dbType;
            _direction = ParameterDirection.Input;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        public SqlParameterItem(string name, ParameterDirection direction, int size = 0)
        {
            _name = name;
            //_dataType = dataType;
            _direction = direction;
            _size = size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        public SqlParameterItem(string name, SqlDbType dbType, ParameterDirection direction, int size = 0)
        {
            _name = name;
            _dbType = dbType;
            _direction = ParameterDirection.Input;
        }

        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get { return _value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ParameterDirection Direction
        {
            get { return _direction; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Size
        {
            get { return _size; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SqlDbType DbType
        {
            get { return _dbType; }
        }
        #endregion
    }
}


