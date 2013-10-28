using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Ifeng.Utility.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
         void LoadData(DataLoader loader);
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DataLoader
    {
        private IDataReader _reader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"></param>
        public DataLoader(IDataReader reader)
        {
            _reader = reader;
        }

        public static DataLoader Create(IDataReader reader)
        {
            if (reader.FieldCount > 20)
                return new ListDataLoader(reader);
            else
                return new DictDataLoader(reader);
        }

        protected abstract int GetOrdinal(string colName);

        /// <summary>
        /// Load Data For Entity
        /// </summary>
        /// <param name="dataRow"></param>
        public void LoadData(IDataRow dataRow)
        {
            dataRow.LoadData(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colName"></param>
        /// <returns></returns>
        public T GetValue<T>(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return default(T);
            return (T)_reader.GetValue(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInt(string colName,int defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetInt32(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public long GetLong(string colName, int defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetInt64(index);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public int? GetNullInt(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetInt32(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(string colName, string defaultValue = "")
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetString(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public float GetFloat(string colName, float defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetFloat(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public float? GetNullFloat(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetFloat(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public double GetDouble(string colName, double defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetDouble(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public double? GetNullDouble(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetDouble(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public short GetShort(string colName, short defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetInt16(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public short? GetNullShort(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetInt16(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public byte GetShort(string colName, byte defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetByte(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public byte? GetNullByte(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetByte(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public byte[] GetBytes(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return (byte[])_reader[colName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public DateTime GetDateTime(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return DateTime.MinValue;
            return _reader.GetDateTime(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public DateTime? GetNullDateTime(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetDateTime(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolean(string colName,bool defaultValue = false)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetBoolean(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public bool? GetNullBoolean(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetBoolean(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public decimal GetDecimal(string colName, decimal defaultValue = 0)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return defaultValue;
            return _reader.GetDecimal(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public decimal? GetNullDecimal(string colName)
        {
            var index = GetOrdinal(colName);
            if (index == -1 || _reader.IsDBNull(index))
                return null;
            return _reader.GetDecimal(index);
        }
    }

    internal class DictDataLoader : DataLoader
    {
        private Dictionary<string, int> dict;

        public DictDataLoader(IDataReader reader)
            : base(reader)
        {
            int count = reader.FieldCount;
            dict = new Dictionary<string, int>(count, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < count; ++i)
            {
                dict[reader.GetName(i)] = i;
            }
        }

        protected override int GetOrdinal(string columnName)
        {
            int index = -1;
            return dict.TryGetValue(columnName, out index) ? index : -1;
        }
    }

    internal class ListDataLoader : DataLoader
    {
        private List<string> list;

        public ListDataLoader(IDataReader reader)
            : base(reader)
        {
            int count = reader.FieldCount;
            list = new List<string>(count);
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.GetName(i));
            }
        }

        protected override int GetOrdinal(string columnName)
        {
            return list.FindIndex(0, r => { return columnName == r; });
        }
    }
}
