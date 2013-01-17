using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.OODSS.Test.Client;
using Simpl.OODSS.Test.Messages;
using Simpl.OODSS.Messages;
using Simpl.Serialization;

namespace Simpl.OODSS.Test.TypesScope
{
    class TestTypesScope
    {
        public const string TypesScopeName = "TestTypesScope";

        public static readonly Type[] TestTypes =
            {
                typeof (TestServiceUpdate),
                typeof (TestServiceRequest),
                typeof (TestServiceResponse)
            };

        public static SimplTypesScope Get()
        {
            return SimplTypesScope.Get(TypesScopeName, DefaultServicesTranslations.Get(), TestTypes);
        }
    }
}
