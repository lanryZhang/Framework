using System.Collections.Generic;

//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 条件语句
    /// </summary>
    public class Where
    {
        internal List<WhereItem> _list = new List<WhereItem>();
        /// <summary>
        /// 条件列表
        /// </summary>
        internal List<WhereItem> List { get { return _list; } }

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public Where()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Where(string name, object value)
        {
            _list.Add(new WhereItem(name, value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="whereType"></param>
        /// <param name="value"></param>
        public Where(string name, WhereType whereType, object value)
        {
            _list.Add(new WhereItem(name, whereType, value));
        }
        #endregion

        #region 组合条件
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where Add(string name, object value)
        {
            _list.Add(new WhereItem(name, value));
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="whereType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where Add(string name, WhereType whereType, object value)
        {
            _list.Add(new WhereItem(name, whereType, value));
            return this;
        } 
        #endregion
    }
}
