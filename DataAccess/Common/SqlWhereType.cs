//

namespace Ifeng.DataAccess.Common
{
    /// <summary>
    /// 树形查询条件
    /// </summary>
    public enum WhereType
    {
        /// <summary>
        /// 
        /// </summary>
        Equal = 0,
        /// <summary>
        /// 
        /// </summary>
        NotEqual = 1,
        /// <summary>
        /// 
        /// </summary>
        GreaterThan = 2,
        /// <summary>
        /// 
        /// </summary>
        LessThan = 3,
        /// <summary>
        /// 
        /// </summary>
        GreaterAndEqual = 4,
        /// <summary>
        /// 
        /// </summary>
        LessAndEqual = 5,
        /// <summary>
        /// 
        /// </summary>
        Like = 6,
        /// <summary>
        /// 
        /// </summary>
        LeftLike = 7,
        /// <summary>
        /// 
        /// </summary>
        RightLike = 8,
        /// <summary>
        /// 
        /// </summary>
        ExplicitLike = 9,
        /// <summary>
        /// 
        /// </summary>
        In = 10
    }
}
