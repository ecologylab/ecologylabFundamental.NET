using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
   /**
     *
     * <configuration>
            <pref_double name="index_thumb_dist" value="200"/>
       </configuration>
     * 
     */


    public class Configuration : ElementState
    {

        [simpl_nowrap]
        [simpl_map("pref_double")]
        public static Dictionary<String, PrefDouble> prefs = new Dictionary<string, PrefDouble>();


        #region GetterSetters

        public Dictionary<String, PrefDouble> Preferences
        {
            get { return prefs; }
            set { prefs = value; }
        }
        #endregion


    }

}
