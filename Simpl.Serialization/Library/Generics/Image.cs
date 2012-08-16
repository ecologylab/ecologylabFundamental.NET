namespace Simpl.Serialization.Library.Generics
{
    using Simpl.Serialization.Attributes;
    
    [SimplInherit]
    public class Image : Media
    {
        
	    [SimplScalar]
	    int	width;

        [SimplScalar]
	    int	height; 
    }
}
