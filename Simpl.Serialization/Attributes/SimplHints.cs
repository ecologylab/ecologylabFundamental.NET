using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplHints : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
         private readonly Hint[] hints;

        /// <summary>
        /// 
        /// </summary>
         /// <param name="pHints"></param>
         public SimplHints(params Hint[] pHints)
        {
            this.hints = pHints;
        }

        /// <summary>
        /// 
        /// </summary>
        public Hint[] Value
        {
            get
            {
                return hints;
            }
        }
    }

    /// <summary>
    /// explicitly defines how scalar types are serialized. 
    /// </summary>
    public enum Hint
    {
        /// <summary>
        /// scalar types serialized as attribute in XML
        /// </summary>
        XmlAttribute,

        /// <summary>
        /// scalar types serialized as leaf in XML
        /// </summary>
        XmlLeaf,

        /// <summary>
        /// scalar types serialized as leaf CDATA
        /// </summary>
        XmlLeafCdata,

        /// <summary>
        /// scalar types serialized as text in XML
        /// </summary>
        XmlText,

        /// <summary>
        /// scalar types serialized as text CDATA in XML
        /// </summary>
        XmlTextCdata,

        /// <summary>
        /// undefined constant (should not be used)
        /// </summary>
        Undefined
    }
}
