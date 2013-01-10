using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Impl
{
    public abstract class Manager:Shutdownable
    {
        private List<Shutdownable> _shutdownDependents = new List<Shutdownable>();

        private EventHandler m_shudDown;

        private event EventHandler shutDown
        {
            add { m_shudDown += value; }
            remove { m_shudDown -= value; }
        }

        private bool _shutdownCalled = false;


        public void Shutdown()
        {
            if (!_shutdownCalled)
            {
                _shutdownCalled = true;

                
            }
        }

        public void AddDependentShutdownable(Shutdownable s)
        {
            _shutdownDependents.Add(s);
        }

        public void AddShutdownEventListener(EventListener e)
        {
            throw new NotImplementedException();
        }

        protected abstract void ShutdownImpl();
    }
}
