using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class ImageSearch<I, X, T> : MediaSearch<X, MediaSearchResult<X>>
        where I : Image
        where X : I
        where T : MediaSearchResult<X>
    {
    }
}
