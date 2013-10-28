//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 查询语句
    /// </summary>
    public class Select
    {
        private string[] _fields;
        private Where _condition = new Where();

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public Where Condition
        {
            set { _condition = value; }
            get { return _condition; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string[] Fields
        {
            set { _fields = value; }
            get { return _fields; }
        }


        internal int PageIndex { set; get; }
        internal int PageSize { set; get; }
        internal string GroupByString { get; private set; }
		internal string OrderByString { get; private set; }
        internal string OverOrderByString { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public Select()
        {
            this.Fields = new string[] { "*" };
            PageIndex = 1;
            PageSize = 25;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        public Select(string[] fields)
        {
            this.Fields = fields;
            PageIndex = 1;
            PageSize = 25;
        }
        #endregion

        #region Where Condition

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Where(string name, object value)
        {
            Where(name, WhereType.Equal, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="whereType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Select Where(string name, WhereType whereType, object value)
        {
            if (Condition == null) Condition = new CollectionWhere();
            this.Condition.List.Add(new WhereItem(name, whereType, value));
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Select GroupBy(string[] fields)
        {
            GroupByString = string.Join(",", fields);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Select OrderBy(string field, SortType sort)
        {
            if (OrderByString == null) OrderByString = string.Format("{0} {1}", field, sort);
            else OrderByString += string.Format(",{0} {1}", field, sort);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Select Page(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            return this;
        }

        public Select OverOrderBy(string fieldName)
        {
            this.OverOrderByString = fieldName;
            return this;
        }
        #endregion

    }
}
