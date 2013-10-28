//using System.Linq;

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 查询语句连接方式
    /// </summary>
    public enum SqlJoinType
    {
        /// <summary>
        /// 左连接
        /// </summary>
        LeftJoin,
        /// <summary>
        /// 右连接
        /// </summary>
        RightJoin,
        /// <summary>
        /// 内连接
        /// </summary>
        InnerJoin,
        /// <summary>
        /// 外连接
        /// </summary>
        OutJoin,
        /// <summary>
        /// 交叉连接
        /// </summary>
        CrossJoin
    }
    
    /// <summary>
    /// 联合方式
    /// </summary>
    public enum SqlUnionType
    {
        /// <summary>
        /// Union
        /// </summary>
        Union,
        /// <summary>
        /// UnionAll
        /// </summary>
        UnionAll
    }
}
