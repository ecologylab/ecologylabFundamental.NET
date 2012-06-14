using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.PlatformSpecifics
{
    class FundamentalPlatformSpecifics
    {
	    private static IFundamentalPlatformSpecifics	iFundamentalPlatformSpecifics;

	    private static Boolean dead	= false;

	    public static void set(IFundamentalPlatformSpecifics that)
	    {
		    iFundamentalPlatformSpecifics = that;
	    }

	    public static IFundamentalPlatformSpecifics Get()
	    {
		    if (dead)
			    throw new Exception("Can't initialize FundamentalPlatformSpecifics");

            if (iFundamentalPlatformSpecifics == null)
            {
                iFundamentalPlatformSpecifics = new FundamentalPlatformSpecificsCSharp();
            }

		    return iFundamentalPlatformSpecifics;
	    }
    }
}
