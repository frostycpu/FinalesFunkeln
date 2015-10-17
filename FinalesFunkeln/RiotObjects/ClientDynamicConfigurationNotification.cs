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
    [SerializedName("com.riotgames.platform.client.dynamic.configuration.ClientDynamicConfigurationNotification")]
    public class ClientDynamicConfigurationNotification:DynamicObject
    {
        [Transient]
        [Hidden]
        public string TypeName { get { return "com.riotgames.platform.client.dynamic.configuration.ClientDynamicConfigurationNotification"; } }

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        [Transient]
        // ReSharper disable once InconsistentNaming
        public Dictionary<string, object> configs { get; private set; }
        
        [SerializedName("delta")]
        public bool? Delta { get; set; }

        [Hidden]
        [SerializedName("configs")]
        public string Configurations
        {
            get 
            {
                return Serializer.Serialize(configs);
            }
            set
            {
                configs = Serializer.Deserialize<Dictionary<string, object>>(value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new[]{"configs","delta","TypeName"};
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch(binder.Name)
            {
                case "configs":
                    result = configs;
                    break;
                case "delta":
                    result = Delta;
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
                case "configs":
                    configs=value as Dictionary<string,object>;
                    break;
                case "delta":
                    Delta = value as bool?;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
