using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Configuration
{
    public class Configuration
    {
        [SimplClasses(new[] {typeof (Pref), typeof (PrefDouble), typeof (PrefInteger)})] 
        [SimplComposite]
        private Pref pref;

        

        [SimplClasses(new[] { typeof(Pref), typeof(PrefDouble), typeof(PrefInteger) })]
        [SimplCollection]
        private List<Pref> prefs;

        public Configuration()
        {
            
        }

        public Configuration(Pref p, List<Pref> ps )
        {
            pref = p;
            prefs = ps;
        }

    }
}
