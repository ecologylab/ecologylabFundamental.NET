using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    class UriType : ReferenceType
    {
        /// <summary>
        /// 
        /// </summary>
        public UriType()
            : base(typeof(Uri))
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            Object result = null;
            try
            {
                if (scalarUnmarshallingContext != null)
                {
                    Uri baseUri = scalarUnmarshallingContext.UriContext();
                    if (baseUri != null)
                        result = new Uri(baseUri, value);
                }
                else
                    result = new Uri(value);
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
