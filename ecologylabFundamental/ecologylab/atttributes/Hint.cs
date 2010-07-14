using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// explicitly defines how scalar types are serialized. 
    /// </summary>
    public enum Hint
    {
        /// <summary>
        /// scalar types serialized as attribute in XML
        /// </summary>
        XML_ATTRIBUTE, 

        /// <summary>
        /// scalar types serialized as leaf in XML
        /// </summary>
        XML_LEAF, 
        
        /// <summary>
        /// scalar types serialized as leaf CDATA
        /// </summary>
        XML_LEAF_CDATA, 
        
        /// <summary>
        /// scalar types serialized as text in XML
        /// </summary>
        XML_TEXT, 
        
        /// <summary>
        /// scalar types serialized as text CDATA in XML
        /// </summary>
        XML_TEXT_CDATA, 
        
        /// <summary>
        /// undefined constant (should not be used)
        /// </summary>
        UNDEFINED
    }
}
