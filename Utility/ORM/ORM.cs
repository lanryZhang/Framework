using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Reflection;
using Ifeng.Utility.Attributes;
using MongoDB.Bson;
using MongoDB;
//


namespace Ifeng.Utility.ORM
{
    /// <summary>
    /// 
    /// </summary>
    public static class ORM
    {
        /// <summary>
        /// 数据表映射为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T TableToObject<T>(IDataReader reader)
        {
            Type type = typeof(T);
            PropertyInfo[] proInfo;
            T en;
            object[] attrs = null;

            DBAttibute propertyAttr;

            en = (T)Activator.CreateInstance(type);
            proInfo = en.GetType().GetProperties();

            foreach (PropertyInfo pinfo in proInfo)
            {
                attrs = pinfo.GetCustomAttributes(true);
                propertyAttr = null;
                if (attrs != null && attrs.Length != 0)
                {
                    
                    foreach (var patt in attrs)
                    {
                        if (patt.GetType() == typeof(DBAttibute))
                        {
                            propertyAttr = (DBAttibute)patt;
                        }
                    }
                    if (!propertyAttr.NoMapping)
                    {
                        var name = propertyAttr.Name == null ? pinfo.Name : propertyAttr.Name;

                        if (propertyAttr != null && reader[name] != DBNull.Value &&
                             reader[pinfo.Name].ToString().ToLower() != "null")
                            pinfo.SetValue(en, reader[name], null);
                    }
                }
               
                if (propertyAttr == null)
                {
                    try
                    {
                        if (reader[pinfo.Name] != DBNull.Value && reader[pinfo.Name].ToString().ToLower() != "null")
                            pinfo.SetValue(en, reader[pinfo.Name], null);
                    }
                    catch
                    { }
                }
            }
            return en;
        }

        /// <summary>
        /// MongoDB Document转化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static T DocumentToObject<T>(Document doc)
        {
            Type type = typeof(T);
            PropertyInfo[] proInfo;
            T en;
            object[] attrs = null;

            DBAttibute propertyAttr;

            en = (T)Activator.CreateInstance(type);
            proInfo = en.GetType().GetProperties();

            foreach (PropertyInfo pinfo in proInfo)
            {
                attrs = pinfo.GetCustomAttributes(true);
                if (attrs != null && attrs.Length != 0)
                {
                    propertyAttr = (DBAttibute)attrs[0];
                    if (doc[propertyAttr.Name] != null)
                        pinfo.SetValue(en, doc[propertyAttr.Name], null);
                    else
                    {
                        
                    }
                }
                else
                {
                    try
                    {
                        pinfo.SetValue(en, doc[pinfo.Name], null);
                    }
                    catch
                    { }
                }
            }
            return en;
        }

        /// <summary>
        /// 将MapReduce返回的Document转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static T MrDocumentToObject<T>(Document doc)
        {
            var newDoc = new Document();
            ConvertComplexDocument(doc, newDoc);

            return DocumentToObject<T>(newDoc);
        }

        private static void ConvertComplexDocument(Document doc,Document newDoc)
        {
            newDoc = newDoc ?? new Document();

            foreach (var key in doc.Keys)
            {
                if (doc[key].GetType() == typeof(Document))
                {
                    var temp = (Document) doc[key];
                    ConvertComplexDocument(temp, newDoc);
                }
                else
                {
                    newDoc.Add(key, doc[key]);
                }
            }
        }

        /// <summary>
        /// 根据实体映射表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ReflectTableName<T>(T entity)
        {
            TableAttribute att = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

            return att.TableName;
        }

        /// <summary>
        /// 根据实体映射字段/值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IDictionary<string,object> ReflectFields<T>(T entity)
        {
            PropertyInfo[] proInfos = entity.GetType().GetProperties();

            DBAttibute filedAttt = null;
            Dictionary<string, object> fields = new Dictionary<string, object>();

            foreach (PropertyInfo info in proInfos)
            {
                filedAttt = (DBAttibute)Attribute.GetCustomAttribute(info, typeof(DBAttibute));
                if (filedAttt != null && filedAttt.NoMapping)
                    continue;

                if (filedAttt != null )
                {
                    if (!filedAttt.IsIdentity)
                        fields.Add(filedAttt.Name, info.GetValue(entity, null));
                }
                else
                {
                    fields.Add(info.Name, info.GetValue(entity, null));
                }
            }

            return fields;
        }

        /// <summary>
        /// 根据实体映射SqlWhere条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReflectSqlWhere<T>(T entity,out object value)
        {
            string name = string.Empty;
            value = null;

            PropertyInfo[] proInfos = entity.GetType().GetProperties();

            DBAttibute filedAttt = null;

            foreach (PropertyInfo info in proInfos)
            {
                filedAttt = (DBAttibute)Attribute.GetCustomAttribute(info, typeof(DBAttibute));

                if (filedAttt != null && filedAttt.IsPrimaryKey)
                {
                   name = info.Name;
                   value = info.GetValue(entity, null);
                }
            }

            return name;
        }
    }
}
