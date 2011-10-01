using System;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
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
            : base(typeof(ParsedUri), CLTypeConstants.JavaParsedUrl, CLTypeConstants.ObjCParsedUrl, null)
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
