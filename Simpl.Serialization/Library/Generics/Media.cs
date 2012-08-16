namespace Simpl.Serialization.Library.Generics
{
    using System;
    using Simpl.Serialization.Attributes;

    public class Media
    {
        [SimplScalar]
	    String data; // base64 encoded
    }
}
