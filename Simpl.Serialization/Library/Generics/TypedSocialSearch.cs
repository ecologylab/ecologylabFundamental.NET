using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;


namespace Simpl.Serialization.Library.Generics
{
    [SimplInherit]
    public class TypedSocialSearch<SSR> : Search<SSR>
        where SSR : SocialSearchResult
    {
	
	    [SimplScalar]
	    public String serviceId;

        [SimplScalar]
	    public long userId;
	
    }
}
