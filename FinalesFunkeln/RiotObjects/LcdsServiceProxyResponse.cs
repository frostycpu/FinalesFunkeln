using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FinalesFunkeln.Controls.Attributes;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace FinalesFunkeln.RiotObjects
{
    [Serializable]
    [SerializedName("com.riotgames.platform.serviceproxy.dispatch.LcdsServiceProxyResponse")]
    public class LcdsServiceProxyResponse:DynamicObject
    {
        [Hidden]
        public string TypeName { get { return "com.riotgames.platform.serviceproxy.dispatch.LcdsServiceProxyResponse"; } }

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        [Transient]
        // ReSharper disable once InconsistentNaming
        public object payload { get; private set; }

        [SerializedName("status")]
        public string Status { get; set; }

        [SerializedName("messageId")]
        public string MessageId { get; set; }

        [SerializedName("methodName")]
        public string MethodName { get; set; }

        [SerializedName("serviceName")]
        public string ServiceName { get; set; }

        [Hidden]
        [SerializedName("payload")]
        public string Payload
        {
            get 
            {
                return payload == null ? null : Serializer.Serialize(payload);
            }
            set
            {
                payload = value == null ? null : Serializer.Deserialize<object>(value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new[] { "payload", "status", "messageId", "methodName", "serviceName", "TypeName" };
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
                    payload = value;
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
                default:
                    return false;
            }
            return true;
        }
    }
}
