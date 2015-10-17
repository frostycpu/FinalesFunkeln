namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class AcknowledgeListItem:PacketListItem
    {
        public string ServiceName { get; set; }
        public string Operation { get; set; }
        public string Title { get { return string.Format("[{0}] Ack {1}.{2}", Time, ServiceName, Operation); } }
        public object[] InvokeArguments { get; set; }
        public object Response { get; set; }

        public AcknowledgeListItem(string serviceName, string operation, object[] args, object response)
        {
            Operation = operation;
            ServiceName = serviceName;
            InvokeArguments = args;
            Response = response;

        }
        
    }
}
