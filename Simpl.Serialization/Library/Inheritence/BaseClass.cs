using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Inheritence
{
    [SimplOtherTags(new string[] {"the_field"})]
    [SimplTag("fred")]
    public class BaseClass
    {
        [SimplTag("new_tag_var")]
        [SimplOtherTags(new string[] {"other_tag_var"})]
        [SimplScalar] private int var = 3;

        public BaseClass()
        {
        }
    }
}
