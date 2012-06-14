using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class Search<T> : ElementState
        where T : SearchResult
    {

	    [SimplScalar]
	    public String query;

	    [SimplCollection]
	    public List<T> searchResults;

    }
}
