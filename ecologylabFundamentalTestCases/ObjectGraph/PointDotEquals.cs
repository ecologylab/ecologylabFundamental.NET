using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    public class PointDotEquals : ElementState 
    {
	    [simpl_scalar]
	    public int x;
	    [simpl_scalar]
	    public int y;

	    public PointDotEquals() {}

	    public PointDotEquals(int x, int y) {
		    this.x = x;
		    this.y = y;
	    }
	
	    public override int GetHashCode() {
		    return x+y;
	    }
	
	    public override bool Equals(Object other) {
		    if (!(other is PointDotEquals)) {
			    return false;
		    }
		    PointDotEquals o = (PointDotEquals)other;
		    return x == o.x && y == o.y;    
	    }
	
	    public override String ToString()
	    {
		    return "["+x+","+y+"]";
	    }
    }
}
