using System;
//

namespace Ifeng.Utility.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage( AttributeTargets.Parameter, AllowMultiple=false)]
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string RegExp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        public ParameterAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="regExp"></param>
        public ParameterAttribute(object defaultValue, string regExp)
        {
            DefaultValue = defaultValue;
            RegExp = regExp;
        }
    }
}
