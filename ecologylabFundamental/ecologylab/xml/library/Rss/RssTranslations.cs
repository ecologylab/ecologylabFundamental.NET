using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.library
{
    public class RssTranslations
    {
        public static Type[] translations = { typeof(RssState), typeof(Channel), typeof(Item) };

        public static TranslationScope Get()
        {
            return TranslationScope.Get("rss", translations);
        }
    }
}
