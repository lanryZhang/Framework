using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Ifeng.DataAccess.NoSql.Redis
{
    public class RedisProvider : RedisClient
    {
        #region Constructor
        public RedisProvider()
        {

        }
        public RedisProvider(string ip, int port)
            : base(ip, port)
        {
        }

        public RedisProvider(string ip, int port, string pwd)
            : base(ip, port, pwd)
        {
        }
        #endregion

        #region  List

        #region Add
        public void AddRangeToList<T>(string listId, T value) where T : IEnumerable
        {
            var newList = new List<string>();

            foreach (var item in value)
            {
                newList.Add(SerializeToString(item));
            }

            base.AddRangeToList(listId, newList);
        }

        public void AddItemToList<T>(string listId, T value)
        {
            base.AddItemToList(listId, SerializeToString(value));
        }
        #endregion

        #region Get
        public List<T> GetAllItemsFromList<T>(string listId)
        {
            var list = base.GetAllItemsFromList(listId);
            return DeserializeFromListString<T>(list);
            
        }

        public T GetItemFromList<T>(string listId, int index)
        {
            var en = base.GetItemFromList(listId, index);
            return DeserializeFromString<T>(en);
        }

        public List<T> GetRangeFromList<T>(string listId, int startingFrom, int endingAt)
        {
            var list = base.GetRangeFromList(listId, startingFrom, endingAt);
            return DeserializeFromListString<T>(list);
        }

        public List<T> GetRangeFromSortedList<T>(string listId, int startingFrom, int endingAt)
        {
            var list = base.GetRangeFromSortedList(listId, startingFrom, endingAt);
            return DeserializeFromListString<T>(list);
        }

        public List<T> GetSortedItemsFromList<T>(string listId, SortOptions sortOptions)
        {
            var list = base.GetSortedItemsFromList(listId, sortOptions);
            return DeserializeFromListString<T>(list);
        }
        #endregion

        #region Pop
        public T BlockingPopItemFromList<T>(string listId, TimeSpan? timeOut)
        {
            var str = base.BlockingPopItemFromList(listId, timeOut);
            return DeserializeFromString<T>(str);
        }

        public T PopAndPushItemBetweenLists<T>(string fromListId, string toListId)
        {
            var str = base.PopAndPushItemBetweenLists(fromListId,toListId);
            return DeserializeFromString<T>(str);
        }

        public T PopItemFromList<T>(string listId)
        {
            var str = base.PopItemFromList(listId);
            return DeserializeFromString<T>(str);
        }
        #endregion

        #region Prepend
        public void PrependItemToList<T>(string listId, T value)
        {
            var str = SerializeToString(value);
            base.PrependItemToList(listId, str);
        }

        public void PrependRangeToList<T>(string listId, List<T> values)
        {
            var list = SerializeToString<T>(values);
            base.PrependRangeToList(listId, list);
        }
        #endregion

        #region Push
        public void PushItemToList<T>(string listId, T value)
        {
            var str = SerializeToString(value);
            base.PushItemToList(listId, str);
        }
        #endregion

        #region Remove
        public int RemoveItemFromList<T>(string listId, T value)
        {
            var str = SerializeToString(value);
            return base.RemoveItemFromList(listId, str);
        }

        public int RemoveItemFromList<T>(string listId, T value, int noOfMatches)
        {
            var str = SerializeToString(value);
            return base.RemoveItemFromList(listId, str, noOfMatches);
        }

        public T RemoveStartFromList<T>(string listId)
        {
            var str = base.RemoveStartFromList(listId);
            return DeserializeFromString<T>(str);
        }
        #endregion

        #region Dequeue
        public T BlockingDequeueItemFromList<T>(string listId, TimeSpan? timeOut)
        {
            var en = base.BlockingDequeueItemFromList(listId, timeOut);
            return DeserializeFromString<T>(en);
        }
        public T DequeueItemFromList<T>(string listId)
        {
            var en = base.DequeueItemFromList(listId);
            return DeserializeFromString<T>(en);
        }
        #endregion

        #region Enqueue
        public void EnqueueItemOnList<T>(string listId, T value)
        {
            var str = SerializeToString(value);
            base.EnqueueItemOnList("", str);
        }
        #endregion

        #endregion

        #region Set

        #region Add
        public void AddItemToSet<T>(string setId, T item)
        {
            var str = SerializeToString<T>(item);
            base.AddItemToSet(setId, str);
        }

        public bool AddItemToSortedSet<T>(string setId, T value)
        {
            var str = SerializeToString<T>(value);
            return base.AddItemToSortedSet(setId, str);
        }

        public bool AddItemToSortedSet<T>(string setId, T value, double score)
        {
            var str = SerializeToString<T>(value);
            return base.AddItemToSortedSet(setId, str, score);
        }

        public bool AddItemToSortedSet<T>(string setId, T value, long score)
        {
            var str = SerializeToString<T>(value);
            return base.AddItemToSortedSet(setId, str, score);
        }

        public void AddRangeToSet<T>(string setId, List<T> items)
        {
            var list = SerializeToString<T>(items);
            base.AddRangeToSet(setId, list);
        }

        public bool AddRangeToSortedSet<T>(string setId, List<T> values, double score)
        {
            var list = SerializeToString<T>(values);
            return base.AddRangeToSortedSet(setId, list, score);
        }

        public bool AddRangeToSortedSet<T>(string setId, List<T> values, long score)
        {
            var list = SerializeToString<T>(values);
            return base.AddRangeToSortedSet(setId, list, score);
        }
        #endregion
        
        #endregion

        #region Private
        private List<T> DeserializeFromListString<T>(List<string> list) 
        {
            var newList = new List<T>();
            foreach (var item in list)
            {
                var en = TypeSerializer.DeserializeFromString<T>(item);
                newList.Add(en);
            }
            return newList;
        }

        private T DeserializeFromString<T>(string en)
        {
            return TypeSerializer.DeserializeFromString<T>(en);
        }

        private string SerializeToString<T>(T value)
        {
            return TypeSerializer.SerializeToString(value);
        }

        private List<string> SerializeToString<T>(List<T> list)
        {
            var newList = new List<string>();

            foreach (var item in list)
            {
                var str = TypeSerializer.SerializeToString(item);
                newList.Add(str);
            }
            return newList;
        }
        #endregion
    }
}
