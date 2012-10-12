using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    class PlatformSpecificTypesWindowsStoreApps
    {
        public static List<ScalarType> ScalarTypes
        {
            get
            {
                return new List<ScalarType>
                           {
                               new FileType(),
                               new RectType(),
                               new ColorType()
                           };

            }
        }

        static PlatformSpecificTypesWindowsStoreApps()
        {
            TypeRegistry.RegisterTypes(ScalarTypes.ToArray());
        }
    }
}
