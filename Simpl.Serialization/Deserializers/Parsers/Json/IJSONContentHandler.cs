using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Deserializers.Parsers.Json
{
    /// <summary>
    /// 
    /// </summary>
    interface IJsonContentHandler
    {
        /// <summary>
        /// 
        /// </summary>
        void StartJson();

        /// <summary>
        /// 
        /// </summary>
        void StartObject();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void StartObjectEntry(String key);

        /// <summary>
        /// 
        /// </summary>
        void StartArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Primitive(Object value);

        /// <summary>
        /// 
        /// </summary>
        void EndJson();

        /// <summary>
        /// 
        /// </summary>
        void EndObject();

        /// <summary>
        /// 
        /// </summary>
        void EndObjectEntry();

        /// <summary>
        /// 
        /// </summary>
        void EndArray();
    }
}
