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
                dictionary.Add(key, value);
            }

            return oldValue;
        }
    }
}
