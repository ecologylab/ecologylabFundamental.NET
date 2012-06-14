using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class MediaSearchResult<M> : SearchResult
        where M : Media
    {

	    [SimplComposite]
	    M media;

	    [SimplComposite]
	    public MediaSearch<M, MediaSearchResult</*java: ? extends*/ M>>	ms;
    }
}
