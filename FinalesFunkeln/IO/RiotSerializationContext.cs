using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtmpSharp.IO;

namespace FinalesFunkeln.IO
{
    class RiotSerializationContext : SerializationContext
    {
        public static readonly RiotSerializationContext Instance = new RiotSerializationContext();
        private RiotSerializationContext()
        {
            Register(typeof(RiotObjects.BroadcastNotification));
            Register(typeof(RiotObjects.ClientSystemStatesNotification));
            Register(typeof(RiotObjects.ClientDynamicConfigurationNotification));
            Register(typeof(RiotObjects.LcdsServiceProxyResponse));
            Register(typeof(RtmpSharp.Messaging.Messages.AcknowledgeMessageExt));
            Register(typeof(RtmpSharp.Messaging.Messages.AsyncMessageExt));
        }
    }
}
