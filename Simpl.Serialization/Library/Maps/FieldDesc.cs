using System;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Library.Maps
{
    public class FieldDes : IMappable<String>
    {
        [SimplScalar] public String fieldName;

        public FieldDes()
        {
            fieldName = "";
        }

        public FieldDes(String fieldName)
        {
            this.fieldName = fieldName;
        }


        public String Key()
        {
            return this.fieldName;
        }
    }
}
