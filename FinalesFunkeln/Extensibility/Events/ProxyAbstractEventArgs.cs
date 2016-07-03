using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility.Events
{
    public abstract class ProxyAbstractEventArgs
    {
        public readonly bool InvokedByProxy;

        protected ProxyAbstractEventArgs(bool invokedByProxy)
        {
            InvokedByProxy = invokedByProxy;
        }
    }
}
