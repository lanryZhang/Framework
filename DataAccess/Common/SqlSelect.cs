//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 查询语句
    /// </summary>
    public class SqlSelect : Select
    {
        private string _table;

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Table
        {
            set { _table = value; }
            get { return _table; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public SqlSelect() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        public SqlSelect(params string[] fields)
            : base(fields)
        {
        }
        #endregion
    }
}
