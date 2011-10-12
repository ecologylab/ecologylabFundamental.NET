using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Items
{
    public class ItemOne : ItemBase
    {
        [SimplScalar] 
        private int testing;


        public ItemOne()
        {

        }

        public ItemOne(int pTesting, int pVar)
        {
            testing = pTesting;
            var = pVar;
        }
    }
}
