using System;
using System.Collections;
using System.Web.Caching;
using System.Web.Hosting;

namespace Ifeng.Utility.Helper
{
    /// <summary>
    /// 缓存，线程安全
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private static Cache _cache;
        /// <summary>
        /// 延期时间
        /// </summary>
        public static double SaveTime { set; get; }

        /// <summary>
        /// Constructor
        /// </summary>
        static CacheHelper()
        {
            _cache = HostingEnvironment.Cache;
            SaveTime = 30.0;
        }

        /// <summary>
        /// 获取某一项值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return _cache.Get(key);
        }

        /// <summary>
        /// 获取某一项值(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            object obj = Get(key);

            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 插入某一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependency"></param>
        /// <param name="priority"></param>
        /// <param name="callBack"></param>
        public static void Insert(string key, object value,
            CacheDependency dependency, 
            CacheItemPriority priority,
            CacheItemRemovedCallback callBack)
        {
            _cache.Insert(key,value,dependency,DateTime.MaxValue,TimeSpan.FromMinutes(SaveTime),  priority,callBack);
        }

        /// <summary>
        /// 插入某一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependency"></param>
        /// <param name="callBack"></param>
        public static void Insert(string key, object value,
            CacheDependency dependency,
            CacheItemRemovedCallback callBack)
        {
            _cache.Insert(key, value, dependency, DateTime.MaxValue, TimeSpan.FromMinutes(SaveTime), CacheItemPriority.Default, callBack);
        }

        /// <summary>
        /// 插入某一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependency"></param>
        public static void Insert(string key, object value,
            CacheDependency dependency)
        {
            _cache.Insert(key, value, dependency, DateTime.MaxValue, TimeSpan.FromMinutes(SaveTime), CacheItemPriority.Default, null);
        }
        
        /// <summary>
        /// 插入某一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Insert(string key, object value)
        {
            _cache.Insert(key, value, null, DateTime.MaxValue, TimeSpan.FromMinutes(SaveTime), CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 删除某一项
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            _cache.Remove(key);
        }

        /// <summary>
        /// 删除所有值
        /// </summary>
        public static void RemoveAll()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();

            while(enumerator.MoveNext())
            {
                _cache.Remove(enumerator.Key.ToString());
            }
        }
    }
}
