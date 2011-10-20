using System.Collections.Generic;
using System.Linq;

namespace Simpl.Fundamental.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class DictionaryList<TK, TV> : Dictionary<TK, TV>, IDictionaryList
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<TV> ArrayList;

        /// <summary>
        /// 
        /// </summary>
        public DictionaryList()
        {
            ArrayList = new List<TV>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DictionaryList(int capacity) : base(capacity)
        {
            ArrayList = new List<TV>(capacity);
        }

        public List<TV> ValuesInList
        {
            get { return ArrayList; }
        }

        #region IDictionaryList Members

        public void Add(object key, object value)
        {
            Add((TK) key, (TV) value);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new void Add(TK key, TV value)
        {
            Remove(key);

            base.Add(key, value);
            ArrayList.Add(value);

            //return oldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool Remove(TK key)
        {
            TV oldValue;

            if (TryGetValue(key, out oldValue))
            {
                base.Remove(key);
                ArrayList.Remove(oldValue);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Clear()
        {
            ArrayList.Clear();
            base.Clear();
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
    }
}