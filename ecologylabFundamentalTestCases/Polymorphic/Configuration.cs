using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ecologylab.serialization;
using ecologylab.attributes;

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
        [simpl_scope("testScope")]
        //[simpl_classes(new Type[] { typeof(Pref), typeof(PrefDouble) })]
        [simpl_map]
        public Dictionary<String, Pref> prefs = new Dictionary<string, Pref>();


        #region GetterSetters

        public Dictionary<String, Pref> Preferences
        {
            get { return prefs; }
            set { prefs = value; }
        }
        #endregion

        internal void fillDictionary()
        {
            PrefDouble prefDouble = new PrefDouble();
            prefDouble.Name = "index_thumb_dist";
            prefDouble.Value = 200;

            Pref pref = new Pref();
            pref.Name = "test_name";

            prefs.Add(prefDouble.Name, prefDouble);
            prefs.Add(pref.Name, pref);
        }
    }
}
