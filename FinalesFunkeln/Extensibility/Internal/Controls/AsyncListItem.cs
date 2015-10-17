using RtmpSharp.IO;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class AsyncListItem:PacketListItem
    {
        public string Title { get { return string.Format("[{0}] Async {1}", Time, GetSimpleTypeName(Body)); } }
        public object Body { get; set; }

        public AsyncListItem(object body)
        {
            Body = body;

        }
        private string GetSimpleTypeName(object obj)
        {
            var o = obj as AsObject;
            return o != null ? o.TypeName : obj.GetType().Name;
        }
    }
}
