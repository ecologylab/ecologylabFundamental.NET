using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using Simpl.Serialization;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    class ParsedUriType : ReferenceType
    {
        /// <summary>
        /// 
        /// </summary>
        public ParsedUriType()
            : base(typeof(ParsedUri))
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            Object result = null;
            try
            {
                ParsedUri baseUri = null;
                if(scalarUnmarshallingContext != null)
                    baseUri = scalarUnmarshallingContext.UriContext();
                if (baseUri != null)
                    result = new ParsedUri(baseUri, value);
                else
                    result = new ParsedUri(value);
            }
            catch (ArgumentNullException e){ }
            catch (ArgumentException e){ }
            catch (UriFormatException e)
            {
                Console.WriteLine(e.Message + " :: " + value);
            }
            return result;
        } 
    }
}
