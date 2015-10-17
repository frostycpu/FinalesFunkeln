using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.IO;

namespace FinalesFunkeln.Lol
{
    public class LolConnection
    {
        private readonly LolProxy _proxy;

        public dynamic AccountSummary { get { return _proxy.AccountSummary; } }
        public dynamic Summoner { get { return _proxy.Summoner; } }
        public string GnChannelId { get { return _proxy.GnChannelId; } }
        public string BcChannelId { get { return _proxy.BcChannelId; } }
        public string CnChannelId { get { return _proxy.CnChannelId; }  }
        internal LolConnection(LolProxy proxy)
        {
            _proxy = proxy;
        }

        public async Task<object> InvokeAsync(string destination, string operation, params object[] arguments)
        {
            return await _proxy.InvokeAsync(destination, operation, arguments);
        }
    }
}
