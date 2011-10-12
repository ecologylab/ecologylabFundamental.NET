using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Items
{
    public class ItemBase
    {
        [SimplScalar]
        protected int var;

        public ItemBase()
        {

        }
    }
}
