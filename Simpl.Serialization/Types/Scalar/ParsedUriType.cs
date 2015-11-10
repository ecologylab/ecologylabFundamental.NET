using System;
using System.Diagnostics;
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
        public override object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            Object result = null;
            try
            {
                ParsedUri baseUri = null;
                if(scalarUnmarshallingContext != null)
                    baseUri = scalarUnmarshallingContext.UriContext;

                result = baseUri != null ? new ParsedUri(baseUri, value) : new ParsedUri(value);
            }
            catch (ArgumentNullException){ }
            catch (ArgumentException){ }
            catch (FormatException e)
            {
                Debug.WriteLine(e.Message + " :: " + value);
            }
            return result;
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            if (context != null && context.BaseUri != null)
            {
                var uri = instance as ParsedUri;
                if (uri != null)
                {
                    var relativeUri = context.BaseUri.MakeRelativeUri(uri);
                    return relativeUri.ToString();
                }
            }
            
            return ((ParsedUri) instance).ToString();
        }

        public override bool SimplEquals(object left, object right)
        {
            return base.GenericSimplEquals<ParsedUri>(left, right);
        }
    }
}