using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.Extensibility;
using FinalesFunkeln.Extensibility.Events;
using FinalesFunkeln.IO;
using RtmpSharp.Messaging;

namespace FinalesFunkeln.Lol
{
    public class LolConnection
    {
        private readonly LolProxy _proxy;

        public dynamic AccountSummary => _proxy.AccountSummary;
        public dynamic Summoner => _proxy.Summoner;
        public string GnChannelId => _proxy.GnChannelId;
        public string BcChannelId => _proxy.BcChannelId;
        public string CnChannelId => _proxy.CnChannelId;

        private ExtensionManager _extensionManager;

        internal LolConnection(LolProxy proxy,ExtensionManager extManager)
        {
            _proxy = proxy;
            _extensionManager = extManager;
        }

        public async Task<object> InvokeAsync(string destination, string operation, params object[] arguments)
        {
            _extensionManager.FireRemoteProcedureCallEvent(new RemoteProcedureCallEventArgs(destination,operation,arguments, true));
            try
            {
                var result = await _proxy.InvokeAsync(destination, operation, arguments);
                _extensionManager.FireAcknowledgeMessageReceivedEvent(new RemoteProcedureCallResponseEventArgs(destination,operation,arguments,result,true));
                return result;
            }
            catch (AggregateException ex) when(ex.InnerException is InvocationException)
            {
                _extensionManager.FireErrorMessageReceivedEvent(new RemoteProcedureCallResponseEventArgs(destination,operation,arguments,ex.InnerException, true));
                throw ex.InnerException;
            }
        }
    }
}
