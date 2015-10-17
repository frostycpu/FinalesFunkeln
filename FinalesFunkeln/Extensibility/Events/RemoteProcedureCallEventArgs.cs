using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility.Events
{
    public class RemoteProcedureCallEventArgs
    {
        public string Destination;
        public string Operation;
        public dynamic[] Parameters;

        public RemoteProcedureCallEventArgs(string destination, string operation, dynamic[] parameters)
        {
            Destination = destination;
            Operation = operation;
            Parameters = parameters;
        }
    }
}
