using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using RtmpSharp.IO;
using RtmpSharp.Messaging;
using RtmpSharp.Messaging.Messages;
using RtmpSharp.Net;

namespace FinalesFunkeln.IO
{
    internal class LolProxy: RtmpProxy
    {
        public string GnChannelId { get; private set; }
        public string BcChannelId { get; private set; }
        public string CnChannelId { get; private set; }

        public dynamic AccountSummary { get; private set; }
        public dynamic Summoner { get; private set; }

        public LolProxy(IPEndPoint source, Uri remote, SerializationContext context, X509Certificate2 cert = null) : base(source, remote, context, cert) 
        {
            AcknowledgeMessageReceived += LolProxy_AcknowledgeMessageReceived;
            CommandMessageReceived += LolProxy_ClientCommandReceived;
        }

        void LolProxy_AcknowledgeMessageReceived(object sender, RemotingMessageReceivedEventArgs args)
        {
            if (args.Destination == "loginService" && args.Operation == "login")
            {
                AccountSummary = ((dynamic)args.Result.Body).accountSummary;
            }
            else if (args.Destination=="clientFacadeService"&&args.Operation=="getLoginDataPacketForUser")
            {
                Summoner = ((dynamic)args.Result.Body).allSummonerData.summoner;
            }
        }

        void LolProxy_ClientCommandReceived(object sender, CommandMessageReceivedEventArgs e)
        {
            if(e.Message.Operation==CommandOperation.Subscribe)
                switch (e.Message.ClientId.Substring(0, 2))
                {
                    case "cn":
                        CnChannelId = e.Message.ClientId;
                        break;
                    case "gn":
                        GnChannelId = e.Message.ClientId;
                        break;
                    case "bc":
                        BcChannelId = e.Message.ClientId;
                        break;
                }
        }
    }
}
