using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using Ecologylab.Collections;
using Simpl.Fundamental.Net;

namespace Simpl.OODSS.Distributed.Impl
{
    public abstract class AbstractServer
    {
        protected SimplTypesScope TranslationScope { get; set; }

        protected Scope<object> ApplicationObjectScope { get; set; }

        /// <summary>
        /// Creates an instance of an NIOServer of some flavor. Creates the backend using the information
        /// in the arguments.
        /// Registers itself as the MAIN_START_AND_STOPPABLE in the object registry.
        /// </summary>
        /// <param name="portNumber"></param>
        /// <param name="ipAddresses"></param>
        /// <param name="requestTranslationScope"></param>
        /// <param name="objectRegistry"></param>
        /// <param name="idleConnectionTimeout"></param>
        /// <param name="maxMessageLength"></param>
        protected AbstractServer(int portNumber, IPAddress[] ipAddresses,
            SimplTypesScope requestTranslationScope, Scope<object> objectRegistry, int idleConnectionTimeout=-1,
            int maxMessageLength=-1) 
        {
            Console.WriteLine("setting up server...");
            TranslationScope = requestTranslationScope;
            ApplicationObjectScope = objectRegistry;
        }

        static readonly Type[] _ourTranslation = {typeof (InitConnectionRequest)};

        public static SimplTypesScope ComposeTranslations(int portNumber, IPAddress ipAddress, 
            SimplTypesScope requestTranslationSpace)
        {
            return ComposeTranslations(_ourTranslation, portNumber, ipAddress, requestTranslationSpace);
        }

        public static SimplTypesScope ComposeTranslations(Type[] newTranslations,
            int portNumber, IPAddress ipAddress, SimplTypesScope requestTranslationSpace, 
            String prefix = "server_base: ")
        {
            return SimplTypesScope.Get(prefix + ipAddress + ":" + portNumber, requestTranslationSpace,
                                       newTranslations);
        }

        protected AbstractServer(int portNumber, IPAddress ipAddress, SimplTypesScope requestTranslationSpace,
                Scope<object> objectRegistry, int idleConnectionTimeout, int maxMessageLength)
            :this(portNumber, new[]{ipAddress}, requestTranslationSpace, 
                objectRegistry, idleConnectionTimeout, maxMessageLength)
        {
        }

        protected abstract BaseSessionManager GenerateContextManager(string seesionId,
                                                                     SimplTypesScope translationScope,
                                                                     Scope<Object> globalScope);


        public void Start()
        {
        }

        public void Stop()
        {
        }

    }
}
