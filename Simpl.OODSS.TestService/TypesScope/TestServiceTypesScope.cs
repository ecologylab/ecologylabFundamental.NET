using System;
using Simpl.OODSS.Messages;
using Simpl.OODSS.TestClientAndMessage.Messages;
using Simpl.Serialization;
using Simpl.OODSS.TestService.Messages;

namespace Simpl.OODSS.TestService.TypesScope
{
    class TestServiceTypesScope
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
