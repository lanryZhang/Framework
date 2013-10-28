using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Ifeng.Utility.Helper;

namespace Ifeng.Utility.Collection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataList<T> : List<T>
    {
        private int _totalCount;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataList()
        { }

        #region Property
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount
        {
            set { _totalCount = value; }
            get { return _totalCount; }
        }
        #endregion

        #region 重写父类Find/FindAll查找函数
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public new T Find(Predicate<T> match)
        {
            ExceptionHelper.TrueThrow(match == null, "匹配条件不能为空.");

            for (int i = 0; i < base.Count; i++)
            {
                if (match(base[i]))
                {
                    return base[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// FindAll
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public new DataList<T> FindAll(Predicate<T> match)
        {
            ExceptionHelper.TrueThrow(match == null, "匹配条件不能为空.");

            DataList<T> list = new DataList<T>();

            for (int i = 0; i < base.Count; i++)
            {
                if (match(base[i]))
                {
                    list.Add(base[i]);
                }
            }

            return list;
        }
        #endregion

        #region IJson 成员
        /// <summary>
        /// 将对象转换为Json对象
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StringWriter sw = new StringWriter())
            {
                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                {
                    jtw.WriteStartObject();

                    jtw.WritePropertyName("TotalCount");
                    jtw.WriteValue(this.TotalCount);

                    jtw.WritePropertyName("Item");
                    serializer.Serialize(jtw, this);

                    jtw.WriteEndObject();

                    return sw.ToString();
                }
            }
        }
        #endregion 
    }
}
