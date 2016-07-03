using System.IO.Packaging;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class AcknowledgeListItem:PacketListItem
    {
        public string ServiceName { get; set; }
        public string Operation { get; set; }
        public string Title => $"[{Time}] Ack {ServiceName}.{Operation}";
        public object[] InvokeArguments { get; set; }
        public object Response { get; set; }
        public bool InvokedByProxy { get; set; }
        public string IconSource => $"pack://application:,,,/Resources/Images/{(InvokedByProxy?"ProxyCall":"")}Ack.png";

        public AcknowledgeListItem(string serviceName, string operation, object[] args, object response, bool invokedByProxy)
        {
            Operation = operation;
            ServiceName = serviceName;
            InvokeArguments = args;
            Response = response;
            InvokedByProxy = invokedByProxy;
        }
        
    }
}
