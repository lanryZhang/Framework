using System;
using System.Collections.Generic;

using System.Text;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 
    /// </summary>
    internal class WhereItem
    {
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        private WhereType _whereType;
        /// <summary>
        /// 
        /// </summary>
        private object _value;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public WhereItem(string name, object value)
        {
            _name = name;
            _whereType = WhereType.Equal;
            _value = value;
            //_conditionName = string.Format("{0}{1}{2}", _name, _whereType, _value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="whereType"></param>
        /// <param name="value"></param>
        public WhereItem(string name, WhereType whereType, object value)
        {
            _name = name;
            _whereType = whereType;
            _value = value;
            //_conditionName = string.Format("{0}{1}{2}", _name, _whereType, _value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 操作符
        /// </summary>
        public WhereType WhereType
        {
            set { _whereType = value; }
            get { return _whereType; }
        }
        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            set { _value = value; }
            get { return _value; }
        }
        ///// <summary>
        ///// 组合条件类型
        ///// </summary>
        //public string ConditionName
        //{
        //    set { _conditionName = value; }
        //    get { return _conditionName; }
        //}
        #endregion
    }
}
