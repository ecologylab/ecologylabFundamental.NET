using System;
using System.Linq;
using System.IO;
using Simpl.Fundamental.PlatformSpecifics;

namespace Simpl.Fundamental.Net
{
    /// <summary>
    /// Extending Uri to support some additional features 
    /// currently include .Domain and .Suffix
    /// </summary>
    public class ParsedUri : Uri
    {
        public const int CONNECT_TIMEOUT    = 15000;

        public const int READ_TIMEOUT       = 25000;

        #region Caches
        
        String _domain;
        String _suffix;
        String _stripped;
        object _file;

        #endregion

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="uri"></param>
        public ParsedUri(ParsedUri baseUri, String uri)
            :base(baseUri, uri)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public ParsedUri(String uri)
            : base(uri)
        {

        }

        /// <summary>
        /// Domain name.
        /// </summary>
        /// <returns></returns>
        public String Domain
        {
            get 
            {
                String result = _domain;
                if (result == null)
                {
                    if (!Host.Contains("."))
                        result = Host;
                    else
                    {
                        //Guaranteed to have an array of atleast length two
                        string[] domains = this.Host.Split('.');
                        result = domains[domains.Length - 2] + "." + domains[domains.Length - 1];
                    }
                    _domain = result;
                }

                return result;
            }
            
        }


        /// <summary>
        /// Returns the file extension suffix of the page, .png, .jsp if any
        /// otherwise returns a blank string ""
        /// </summary>
        /// <returns></returns>
        public String Suffix
        {
            get 
            {
                String result = _suffix;
                if (result == null)
                {
                    //Use System.Uri vars to help
                    String lastSegment = Segments[Segments.Length - 1];
                    int lastIndexOfDot = lastSegment.LastIndexOf('.');
                    result = lastIndexOfDot > -1 ? lastSegment.Substring(lastIndexOfDot + 1) : string.Empty;
                    _suffix = result;
                }
                return result;
            }
            
        }

        /// <summary>
        /// Syntactic sugar and lazy eval on System.Uri.GetLeftPart(UriPartial.Path) 
        /// </summary>
        public String Stripped 
        {
            get 
            {
                string result = _stripped;
                if (result == null)
                {
                    result = FundamentalPlatformSpecifics.Get().GetUriLeftPart(this);
                    _stripped = result;
                }

                return result; 
            }
        }

        public object File
        {
            get
            {
                object result = _file;
                if (result == null && this.IsFile)
                {
                    result = FundamentalPlatformSpecifics.Get().CreateFile(this.LocalPath); 
                    _file = result;
                }
                return result;
            }
        }

        public override string ToString()
        {
            return AbsoluteUri;
        }
    }
}
