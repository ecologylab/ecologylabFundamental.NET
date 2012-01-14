using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simpl.Fundamental.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class DictionaryList<TK, TV> : IDictionary<TK, TV>, IDictionary
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<TV> ArrayList;

        protected Dictionary<TK, TV> Dictionary;

        /// <summary>
        /// 
        /// </summary>
        public DictionaryList()
        {
            ArrayList   = new List<TV>();
            Dictionary  = new Dictionary<TK, TV>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DictionaryList(int capacity)
        {
            ArrayList   = new List<TV>(capacity);
            Dictionary  = new Dictionary<TK, TV>(capacity);
        }

        public List<TV> ValuesInList
        {
            get { return ArrayList; }
        }

        #region IDictionaryList Members

        public bool Contains(object key)
        {
            return Dictionary.ContainsKey((TK) key);
        }

        public void Add(object key, object value)
        {
            Add((TK) key, (TV) value);
        }

        #endregion

        public bool ContainsKey(TK key)
        {
            return Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Add(TK key, TV value)
        {
            Dictionary.Remove(key);

            Dictionary.Add(key, value);
            ArrayList.Add(value);

            //return oldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TK key)
        {
            TV oldValue;

            if (Dictionary.TryGetValue(key, out oldValue))
            {
                Dictionary.Remove(key);
                ArrayList.Remove(oldValue);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public TV this[TK key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        public ICollection<TK> Keys
        {
            get { return Dictionary.Keys; }
        }

        ICollection IDictionary.Values
        {
            get { return ArrayList; }
        }

        ICollection IDictionary.Keys
        {
            get { return Dictionary.Keys; }
        }

        public ICollection<TV> Values
        {
            get { return ArrayList; }
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ArrayList.Clear();
            Dictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            Remove((TK) key);
        }

        object IDictionary.this[object key]
        {
            get { return Dictionary[(TK) key]; }
            set { Dictionary[(TK) key] = (TV) value; }
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            return Remove(item.Key);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return ArrayList.Count; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TV ElementAt(int index)
        {
            return ArrayList.ElementAt(index);
        }

        public void PutAll(DictionaryList<TK, TV> otherList)
        {
            foreach (TK key in otherList.Keys)
            {
                Add(key, otherList[key]);
            }
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}