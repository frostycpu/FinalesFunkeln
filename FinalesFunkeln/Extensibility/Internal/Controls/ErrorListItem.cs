namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class ErrorListItem:CallResponseListItem
    {
        public ErrorListItem(string serviceName, string operation, object[] args, object error, bool invokedByProxy):base("Error", "Err.png", serviceName,operation,args,error,invokedByProxy)
        {

        }
        
    }
}
