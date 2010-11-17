using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ecologylab.net;

namespace ecologylab.serialization
{
    /// <summary>
    /// The handler returns a context used by scalar types to resolve ambiguities, 
    /// specifically relative paths.
    /// </summary>
    public interface IScalarUnmarshallingContext
    {
        ParsedUri UriContext();
    }
}
