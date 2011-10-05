using Simpl.Fundamental.Net;

namespace Simpl.Serialization.Context
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
