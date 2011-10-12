using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Collection
{
    class ClassB
    {
        [SimplScalar] private int w;

        [SimplScalar] private int u;


        public ClassB()
        {
            w = 55;
            u = 54;
        }
    }
}
