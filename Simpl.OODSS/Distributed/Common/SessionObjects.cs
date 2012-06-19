using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using ecologylab.serialization;

namespace Simpl.OODSS.Distributed.Common
{
    /// <summary>
    /// Constants that define general ecologylab objects that get stored in the
    /// Session ObjectRegistry.
    /// </summary>
    static class SessionObjects
    {
        public static readonly string MAIN_START_AND_STOPPABLE = "main_start_and_stoppable";

        public static readonly string MAIN_SHUTDOWNABLE = "main_shutdownable";

        public static readonly SimplTypesScope BROWSER_SERVICES_TRANSLATIONS = OODSSMessages.Get(); ???

        
    }
}
