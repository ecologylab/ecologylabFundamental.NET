using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Configuration
{
    public class Pref
    {
        [SimplScalar] private String name;

        public Pref()
        {
            
        }

        public Pref(String pName)
        {
            name = pName;
        }
    }
}
