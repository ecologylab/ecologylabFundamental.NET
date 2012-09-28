using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpl.Fundamental.PlatformSpecifics
{
    public class FundamentalPlatformSpecifics
    {
        private static IFundamentalPlatformSpecifics _iFundamentalPlatformSpecifics;

        private static Boolean _dead = false;

        private static readonly object SyncLock = new object();

        public static void Set(IFundamentalPlatformSpecifics that)
        {
            _iFundamentalPlatformSpecifics = that;
        }

        public static IFundamentalPlatformSpecifics Get()
        {
            if (_dead)
                throw new Exception("Can't initialize FundamentalPlatformSpecifics");

            if (_iFundamentalPlatformSpecifics == null)
            {
                lock (SyncLock)
                {
                    
                    if (_iFundamentalPlatformSpecifics == null)
                    {
                        string typeName = "Simpl.Fundamental.PlatformSpecifics.FundamentalPlatformSpecificsImpl, Simpl.Fundamental.DotNet";
                        Type platformSpecificsType = Type.GetType(typeName);
                        if (platformSpecificsType == null)
                        {
                            typeName = "Simpl.Fundamental.PlatformSpecifics.FundamentalPlatformSpecificsImpl, Simpl.Fundamental.WindowsStoreApps";
                            platformSpecificsType = Type.GetType(typeName);
                        }
                        if (platformSpecificsType == null)
                        {
                            _dead = true;
                            throw new Exception("Can't initialize FundamentalPlatformSpecifics");
                        }
                        _iFundamentalPlatformSpecifics = (IFundamentalPlatformSpecifics)Activator.CreateInstance(platformSpecificsType);
                    }
                }
            }

            return _iFundamentalPlatformSpecifics;
        }
    }
}
