using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.PlatformSpecifics
{
    class OODSSPlatformSpecifics
    {
        private static IOODSSPlatformSpecifics _iOODSSPlatformSpecifics;

        private static Boolean _dead = false;

        private static readonly object SyncLock = new object();

        public static void Set(IOODSSPlatformSpecifics that)
        {
            _iOODSSPlatformSpecifics = that;
        }

        public static IOODSSPlatformSpecifics Get()
        {
            if (_dead)
                throw new Exception("Can't initialize OODSSPlatformSpecifics");

            if (_iOODSSPlatformSpecifics == null)
            {
                lock (SyncLock)
                {
                    if (_iOODSSPlatformSpecifics ==null)
                    {
                        string typeName = "Simpl.OODSS.PlatformSpecifics.OODSSPlatformSpecificsImpl, Simpl.OODSS.DotNet";
                        Type platformSpecificsType = Type.GetType(typeName);
                        if (platformSpecificsType == null)
                        {
                            typeName = "Simpl.OODSS.PlatformSpecifics.OODSSPlatformSpecificsImpl, Simpl.OODSS.WindowsStoreApps";
                            platformSpecificsType = Type.GetType(typeName);
                        }
                        if (platformSpecificsType == null)
                        {
                            _dead = true;
                            throw new Exception("Can't initialize OODSSPlatformSpecifics");
                        }
                        _iOODSSPlatformSpecifics = (IOODSSPlatformSpecifics)Activator.CreateInstance(platformSpecificsType);
                    }
                }
            }

            return _iOODSSPlatformSpecifics;
        }
    }
}
