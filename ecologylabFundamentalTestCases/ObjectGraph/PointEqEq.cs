using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;
using ecologylabFundamental.ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    [simpl_use_equals_equals]
    public class PointEqEq : ElementState 
    {
	    [simpl_scalar]
	    public int x;

	    [simpl_scalar]
	    public int y;

	    public PointEqEq() {}

	    public PointEqEq(int x, int y) {
		    this.x = x;
		    this.y = y;
	    }
	
	    public override int GetHashCode() {
		    return x+y;
	    }
	
	    public override bool Equals(Object other) {
		    if (!(other is PointEqEq)) {
			    return false;
		    }
		    PointEqEq o = (PointEqEq)other;
		    return x == o.x && y == o.y;
	    }
    }
}
