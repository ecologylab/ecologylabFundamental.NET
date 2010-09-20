using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
    class Class1
    {

    }
    /**
     *
     * <configuration>
            <pref_double name="index_thumb_dist" value="200"/>
       </configuration>
     * 
     */

    public class Pref : ElementState, IMappable
    {
        [simpl_scalar]
        public String name;

        public Pref() { }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public object key()
        {
            return name;
        }
    }

    public class PrefDouble : IMappable // : Pref
    {
        [simpl_scalar]
        double value;

        [simpl_scalar]
        public String name;

        public PrefDouble() { }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public object key()
        {
            return name;
        }

    }

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
