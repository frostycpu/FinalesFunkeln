namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class ErrorListItem:PacketListItem
    {
        public string ServiceName { get; set; }
        public string Operation { get; set; }
        public string Title => $"[{Time}] Error {ServiceName}.{Operation}";
        public object[] InvokeArguments { get; set; }
        public object Error { get; set; }
        public bool InvokedByProxy { get; set; }
        public string IconSource => $"pack://application:,,,/Resources/Images/{(InvokedByProxy ? "ProxyCall" : "")}Err.png";

        public ErrorListItem(string serviceName, string operation, object[] args, object error, bool invokedByProxy)
        {
            Operation = operation;
            ServiceName = serviceName;
            InvokeArguments = args;
            Error = error;
            InvokedByProxy = invokedByProxy;
        }
        
    }
}
