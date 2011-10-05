namespace Simpl.Serialization.Attributes
{
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
