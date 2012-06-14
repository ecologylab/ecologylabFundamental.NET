using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;


namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class SocialSearch : Search<SocialSearchResult>
    {

        [SimplScalar]
	    public long userId;
	
    }
}
