using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class MediaSearch<M, T> : Search<T>
        where M : Media
        where T : MediaSearchResult<M> //java: ? extends M
    {

        [SimplComposite]
	    MediaSearchResult<Media>	firstResult;

    }
}
