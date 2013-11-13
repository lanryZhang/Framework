using System;
//

namespace Ifeng.Utility.Attributes
{
    /// <summary>
    /// 表属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true,AllowMultiple=true)]
    public class TableAttribute : Attribute
    {
        private string desciption;
        private string tableName;

        /// <summary>
        /// 
        /// </summary>
        public TableAttribute()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TableAttribute(string name)
        {
            tableName = name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public TableAttribute(string name, string desc)
        {
            tableName = name;
            desciption = desc;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            set { tableName = value; }
            get { return tableName; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set { desciption = value; }
            get { return desciption; }
        }
    }
}
