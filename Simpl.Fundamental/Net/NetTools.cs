using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Simpl.Fundamental.Net
{
    public class NetTools
    {
        public static IPAddress[] WrapSingleAddress(IPAddress address)
        {
            IPAddress[] wrappedAddress = {address};
            return wrappedAddress;
        }

        public static IPAddress[] GetAllIPAddressesForLocalhost()
        {
            return Dns.GetHostAddresses(Dns.GetHostName());
        }
    }
}
