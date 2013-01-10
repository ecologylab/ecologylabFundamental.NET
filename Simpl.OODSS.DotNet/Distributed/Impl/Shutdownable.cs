using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Impl
{
    public interface Shutdownable
    {
        /// <summary>
        /// Causes this to start to shutdown, and fires a SHUTTING_DOWN event to all
        /// listeners.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// This method allows another application to indicate its dependence on this
	    /// to be shutdown. That is, when this's shutdown() method is called, it
	    /// should call the shutdown() method on each component that depends on it.
	    /// 
	    /// Implementors and callers should take care not to create an infinite loop
	    /// of shutdown() calls through this method. 
        /// </summary>
        /// <param name="s"></param>
        void AddDependentShutdownable(Shutdownable s);

        /// <summary>
        /// This method allows other components to be notified when the shutdown()
	    /// method is called. Listeners will get an ActionEvent whose action command
	    /// is SHUTTING_DOWN.
        /// </summary>
        /// <param name="e"></param>
        void AddShutdownEventListener(EventListener e);

    }
}
