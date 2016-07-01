using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FinalesFunkeln.Controls.Attributes;
using FinalesFunkeln.Util;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace FinalesFunkeln.RiotObjects
{
    [Serializable]
    [SerializedName("com.riotgames.platform.serviceproxy.dispatch.LcdsServiceProxyResponse")]
    public class LcdsServiceProxyResponse:DynamicObject
    {
        [Hidden]
        public string TypeName => "com.riotgames.platform.serviceproxy.dispatch.LcdsServiceProxyResponse";

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();


        [Transient]
        private string _payload;
        [Transient]
        private object _decompressedPayload;

        [Transient]
        // ReSharper disable once InconsistentNaming
        public object payload => _decompressedPayload ?? (_decompressedPayload = Serializer.Deserialize<object>(CompressedPayload? Encoding.UTF8.GetString(Gzip.Decompress(Convert.FromBase64String(_payload))):_payload));

        [Hidden]
        [SerializedName("payload")]
        public string Payload //'payload' has to be declared BEFORE 'compressedPayload' otherwise the client has trouble decoding the payload
        {
            get
            {
                if (_decompressedPayload == null)
                    return _payload;
                string pl;
                try
                {
                    pl = Serializer.Serialize(_decompressedPayload);
                }
                catch (Exception)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    pl = payload as string;
                }
                pl = CompressedPayload ? Convert.ToBase64String(Gzip.Compress(Encoding.UTF8.GetBytes(pl))).Replace("\\u0027", "'") : pl;
                return pl;
            }
            set
            {
                _payload = value;
                _decompressedPayload = null;
            }
        }

        [SerializedName("status")]
        public string Status { get; set; }

        [SerializedName("messageId")]
        public string MessageId { get; set; }

        [SerializedName("methodName")]
        public string MethodName { get; set; }

        [SerializedName("serviceName")]
        public string ServiceName { get; set; }

        [SerializedName("compressedPayload")]
        public bool CompressedPayload { get; set; }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new[] { "payload", "status", "messageId", "methodName", "serviceName", "TypeName", "compressedPayload" };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch (binder.Name)
            {
                case "payload":
                    result = payload;
                    break;
                case "status":
                    result = Status;
                    break;
                case "messageId":
                    result = MessageId;
                    break;
                case "methodName":
                    result = MethodName;
                    break;
                case "serviceName":
                    result = ServiceName;
                    break;
                case "TypeName":
                    result = TypeName;
                    break;
                case "compressedPayload":
                    result = CompressedPayload;
                    break;
                default:
                    result = null;
                    return false;
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            switch (binder.Name)
            {
                case "payload":
                    _decompressedPayload= value;
                    break;
                case "status":
                    Status = value as string;
                    break;
                case "messageId":
                    MessageId = value as string;
                    break;
                case "methodName":
                    MethodName = value as string;
                    break;
                case "serviceName":
                    ServiceName = value as string;
                    break;
                case "compressedPayload":
                    CompressedPayload = (bool)value;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
