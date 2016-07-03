using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility.Events
{
    public class RemoteProcedureCallResponseEventArgs:RemoteProcedureCallEventArgs
    {
        public dynamic ResponseBody;

        public RemoteProcedureCallResponseEventArgs( string destination, string operation, dynamic[] parameters, dynamic responseBody, bool invokedByProxy=false)
            : base(destination, operation, parameters,invokedByProxy)
        {
            ResponseBody = responseBody;
        }
    }
}
