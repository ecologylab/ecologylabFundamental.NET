using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    public class ListDotEquals : ElementState 
    {
	    [simpl_collection("points")]
	    public List<PointDotEquals> points = new List<PointDotEquals>();
	
	    public ListDotEquals() {}
    }
}
