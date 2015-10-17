using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Dynamic;

namespace FinalesFunkeln.RiotObjects
{
    [Serializable]
    public class ExternalizableJsonObject:AsObject, IExternalizable
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        static ExternalizableJsonObject()
        {
            Serializer.RegisterConverters(new[] { new JsonConverter() });
        }


        public ExternalizableJsonObject(string typeName) : base(typeName) { }
        public ExternalizableJsonObject() { }

        public void ReadExternal(IDataInput input)
        {
            int size = input.ReadByte() << 24 | input.ReadByte() << 16 | input.ReadByte() << 8 | input.ReadByte();
            string json=Encoding.UTF8.GetString(input.ReadBytes(size));

            Dictionary<string,object> d=Serializer.Deserialize<Dictionary<string,object>>(json);
            foreach (var kv in d)
            {
                this[kv.Key] = kv.Value;
            }
        }

        public void WriteExternal(IDataOutput output)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var x in this)
                dict[x.Key] = x.Value;
            string json = Serializer.Serialize(dict);
            byte[] b = Encoding.UTF8.GetBytes(json);
            output.WriteByte((byte)(b.Length >> 24));
            output.WriteByte((byte)(b.Length >> 16));
            output.WriteByte((byte)(b.Length >> 8));
            output.WriteByte((byte)(b.Length));
            output.WriteBytes(b);
            
        }
    }

    class JsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new AsObject((Dictionary<string, object>)dictionary);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (obj is Dictionary<string, object>)
                return obj as Dictionary<string, object>;
            var obj2 = (IDictionary<string, object>)obj;
            return obj2.ToDictionary(x => x.Key, x => x.Value);
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(AsObject), typeof(ExpandoObject) }; }
        }
    }
}
