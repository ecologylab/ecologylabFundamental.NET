using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    public class ListEqEq : ElementState 
    {
	    [simpl_collection("points")]
	    public List<PointEqEq> points = new List<PointEqEq>();
	
	    public ListEqEq() {}
    }
}
