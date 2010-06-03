using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.library
{
    /// <summary>
    /// 
    /// </summary>
    public class RssTranslations
    {
        /// <summary>
        /// 
        /// </summary>
        public static Type[] translations = { typeof(RssState), typeof(Channel), typeof(Item) };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static TranslationScope Get()
        {
            return TranslationScope.Get("rss", translations);
        }
    }
}
