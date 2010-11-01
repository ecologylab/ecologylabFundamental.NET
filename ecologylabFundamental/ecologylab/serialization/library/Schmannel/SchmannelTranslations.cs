using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.library.Schmannel
{
    /// <summary>
    /// 
    /// </summary>
    public class SchmannelTranslations
    {
        /// <summary>
        /// 
        /// </summary>
        public static Type[] translations = { typeof(RssState), typeof(Channel), typeof(Item), typeof(Schmannel), typeof(SchmItem), typeof(BItem) };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static TranslationScope Get()
        {
            return TranslationScope.Get("schm_rss", translations);
        }
    }
}
