﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.net
{
    /// <summary>
    /// Extending Uri to support some additional features 
    /// currently include .Domain and .Suffix
    /// </summary>
    public class ParsedUri : Uri
    {

        String domain = null;
        String suffix = null;
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
        public String Domain()
        {
            String result = domain;
            if (result == null)
            {
                if (!Host.Contains('.'))
                    result = Host;
                else 
                {
                    //Guaranteed to have an array of atleast length two
                    string[] domains = this.Host.Split('.');
                    result = domains[domains.Length - 2] + "." + domains[domains.Length - 1];
                }
                domain = result;
            }

            return result;
        }


        /// <summary>
        /// Returns the file extension suffix of the page, .png, .jsp if any
        /// otherwise returns a blank string ""
        /// </summary>
        /// <returns></returns>
        public String Suffix()
        {
            String result = suffix;
            if (result == null)
            {
                //Use System.Uri vars to help
                String lastSegment = Segments[Segments.Length - 1];
                int lastIndexOfDot = lastSegment.LastIndexOf('.');
                if (lastIndexOfDot > -1)
                    result = lastSegment.Substring(lastIndexOfDot + 1);
                else
                    result = "";
                suffix = result;
            }
            return result;
        }
    }
}
