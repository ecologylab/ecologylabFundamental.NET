using System.Collections.Generic;

namespace Simpl.Fundamental.Generic
{
    /// <summary>
    /// Extension to provide a more Java like put() method to dictionary. 
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// creates or update and existing value for the key &. return the old value if there existed, 
        /// else null or default value is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TValue Put<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             TValue value)
        {
            TValue oldValue;

            if (dictionary.TryGetValue(key, out oldValue))
            {
                dictionary[key] = value;
            }
            else
            {
                oldValue = value;
                dictionary.Add(key, value);
            }

            return oldValue;
        }

        /// <summary>
        /// Helper add method which requires instantiation of a List<typeparamref name="TValue"/> if required.
        /// Implementation resembles something like defaultdict(list) in python
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddToList<TKey, TValue>
            (this IDictionary<TKey, List<TValue>> dictionary,
                TKey key,
                TValue value)
        {
            if ( (!typeof(TKey).IsValueType && key == null) || (!typeof(TKey).IsValueType && value == null))
                return;

            if(!dictionary.ContainsKey(key))
            {
                List<TValue> valueList = new List<TValue>();
                dictionary.Add(key, valueList);
            }
            dictionary[key].Add(value);
        }

        /// <summary>
        /// See <see cref="AddToList{TKey,TValue}"/>.
        /// 
        /// NOTE This method errors out silently if the key or the value doesn't exist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void RemoveFromList<TKey, TValue>
            (this IDictionary<TKey, List<TValue>> dictionary,
                TKey key,
                TValue value)
        {
            if ((!typeof(TKey).IsValueType && key == null) || (!typeof(TKey).IsValueType && value == null))
                return;

            if (!dictionary.ContainsKey(key) || !dictionary[key].Contains(value))
                return;

            dictionary[key].Remove(value);
        }

        public static bool IsEmpty<TKey, TValue>
            (this IDictionary<TKey, List<TValue>> dictionary,
                TKey key)
        {
            return dictionary.ContainsKey(key) && dictionary[key].Count == 0;
        }
    }
}
