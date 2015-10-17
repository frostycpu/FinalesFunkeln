using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility.Events
{
    public class AsyncMessageEventArgs
    {
        public dynamic Body;
        public AsyncMessageEventArgs(dynamic body)
        {
            Body = body;
        }
    }
}
