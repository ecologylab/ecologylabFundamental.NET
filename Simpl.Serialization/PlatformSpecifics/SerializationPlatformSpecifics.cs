using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.PlatformSpecifics
{
    class SerializationPlatformSpecifics
    {
	    private static ISerializationPlatformSpecifics	_iSerializationPlatformSpecifics;

	    private static Boolean _dead	= false;

        private static readonly object SyncLock = new object();

	    public static void Set(ISerializationPlatformSpecifics that)
	    {
		    _iSerializationPlatformSpecifics = that;
	    }

	    public static ISerializationPlatformSpecifics Get()
	    {
		    if (_dead)
			    throw new Exception("Can't initialize SerializationPlatformSpecifics");

            if (_iSerializationPlatformSpecifics == null)
            {
                lock (SyncLock)
                {
                    if (_iSerializationPlatformSpecifics == null)
                    {
                        string typeName = "Simpl.Serialization.PlatformSpecifics.SerializationPlatformSpecificsImpl, Simpl.Serialization.DotNet";
                        Type platformSpecificsType = Type.GetType(typeName);
                        if (platformSpecificsType == null)
                        {
                            typeName = "Simpl.Serialization.PlatformSpecifics.SerializationPlatformSpecificsImpl, Simpl.Serialization.WindowsStoreApps";
                            platformSpecificsType = Type.GetType(typeName);
                        }
                        if (platformSpecificsType == null)
                        {
                            _dead = true;
                            throw new Exception("Can't initialize SerializationPlatformSpecifics");
                        }
                        _iSerializationPlatformSpecifics = (ISerializationPlatformSpecifics)Activator.CreateInstance(platformSpecificsType);
                    }
                }
            }

		    return _iSerializationPlatformSpecifics;
	    }
    }
}
