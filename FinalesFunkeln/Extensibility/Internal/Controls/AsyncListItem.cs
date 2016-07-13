using FinalesFunkeln.RiotObjects;
using RtmpSharp.IO;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class AsyncListItem : PacketListItem
    {
        public string Title => $"[{Time}] Async {GetSimpleTypeName(Body)}";
        public object Body { get; set; }

        public AsyncListItem(object body)
        {
            Body = body;
        }

        private string GetSimpleTypeName(object obj)
        {
            var o = obj as AsObject;
            var proxyRes = obj as LcdsServiceProxyResponse;
            if (o == null && proxyRes != null)
            {
                return $"LcdsServiceProxyResponse({proxyRes.ServiceName}.{proxyRes.MethodName})";
            }

            return o != null ? o.TypeName : obj.GetType().Name;
        }
    }
}
