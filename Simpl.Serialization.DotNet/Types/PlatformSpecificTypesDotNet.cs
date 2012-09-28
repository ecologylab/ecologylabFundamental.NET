using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    class PlatformSpecificTypesDotNet
    {
        public static List<ScalarType> ScalarTypes
        {
            get
            {
                return new List<ScalarType>
                           {
                               new FileType(),
                               new RectType()
                           };

            }
        }

        static PlatformSpecificTypesDotNet()
        {
            TypeRegistry.RegisterTypes(ScalarTypes.ToArray());
        }
    }
}
