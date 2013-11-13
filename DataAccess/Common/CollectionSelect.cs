//

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 查询语句
    /// </summary>
    public class CollectionSelect : Select
    {
        private MgCollection _table;

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public MgCollection Collection
        {
            set { _table = value; }
            get { return _table; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public CollectionSelect() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        public CollectionSelect(string[] fields) : base (fields)
        {
        }
        #endregion
    }
}
