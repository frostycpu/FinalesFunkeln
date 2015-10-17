namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class ErrorListItem:PacketListItem
    {
        public string ServiceName { get; set; }
        public string Operation { get; set; }
        public string Title { get { return string.Format("[{0}] Error {1}.{2}", Time, ServiceName, Operation); } }
        public object[] InvokeArguments { get; set; }
        public object Error { get; set; }

        public ErrorListItem(string serviceName, string operation, object[] args, object error)
        {
            Operation = operation;
            ServiceName = serviceName;
            InvokeArguments = args;
            Error = error;

        }
        
    }
}
