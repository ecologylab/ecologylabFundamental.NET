using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Collection
{
    public class ClassA
    {
        [SimplScalar] private int w;

        [SimplScalar] private int u;


        public ClassA()
        {
            w = 55;
            u = 54;
        }
    }
}
