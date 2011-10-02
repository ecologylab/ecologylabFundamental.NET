using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization
{
    public static class DictionaryExtension
    {
        public static TValue Put<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             TValue value)
        {
            TValue oldValue;

            if(dictionary.TryGetValue(key, out oldValue))
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
