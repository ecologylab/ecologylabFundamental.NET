using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    public class Media
    {
        [SimplScalar]
	    String data; // base64 encoded
    }
}
