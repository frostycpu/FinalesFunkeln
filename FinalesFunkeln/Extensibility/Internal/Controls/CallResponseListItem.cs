namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal abstract class CallResponseListItem:PacketListItem
    {
        public string ServiceName { get; set; }
        public string Operation { get; set; }
        public object[] InvokeArguments { get; set; }
        public object Response { get; set; }
        public bool InvokedByProxy { get; set; }
        public string IconSource => $"pack://application:,,,/Resources/Images/{(InvokedByProxy ? "ProxyCall" : "")}{_iconName}";
        public string Title => $"[{Time}] {_status} {(IsLcdsProxyCall ? $"{InvokeArguments[1]}.{InvokeArguments[2]} <- " : "")}{ServiceName}.{Operation}";

        private bool IsLcdsProxyCall => ServiceName == "lcdsServiceProxy" && Operation == "call" && InvokeArguments.Length == 4;
        private string _status;
        private string _iconName;


        protected CallResponseListItem(string status, string iconName, string serviceName, string operation, object[] args, object response, bool invokedByProxy)
        {
            Operation = operation;
            ServiceName = serviceName;
            InvokeArguments = args;
            Response = response;
            InvokedByProxy = invokedByProxy;
            _status = status;
            _iconName = iconName;
        }
        
    }
}
