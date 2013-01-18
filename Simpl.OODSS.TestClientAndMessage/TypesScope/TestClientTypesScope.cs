using System;
using Simpl.OODSS.Messages;
using Simpl.OODSS.TestClientAndMessage.Messages;
using Simpl.Serialization;

namespace Simpl.OODSS.TestClientAndMessage.TypesScope
{
    public class TestClientTypesScope
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
