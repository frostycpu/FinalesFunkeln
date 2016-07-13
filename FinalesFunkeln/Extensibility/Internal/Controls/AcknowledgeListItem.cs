using System.IO.Packaging;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class AcknowledgeListItem:CallResponseListItem
    {
        public AcknowledgeListItem(string serviceName, string operation, object[] args, object response, bool invokedByProxy) : base("Ack", "Ack.png", serviceName, operation, args, response, invokedByProxy)
        {
        }
        
    }
}
