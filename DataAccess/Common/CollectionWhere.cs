using System.Collections.Generic;

//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 条件语句
    /// </summary>
    public class CollectionWhere : Where
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where Where(string name, object value)
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
        public Where Where(string name, WhereType whereType, object value)
        {
            _list.Add(new WhereItem(name, whereType, value));
            return this;
        }
    }
}
