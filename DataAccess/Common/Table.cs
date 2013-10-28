using System.Text;

//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// Table
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 
        /// </summary>
        private string _tableName;


        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
        }

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
        {
            this._tableName = name;
        }

        public Table()
        { }
        #endregion
    }
}
