using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FinalesFunkeln.IO
{
    public class YamlFile : DynamicObject, IDynamicMetaObjectProvider,IEnumerable<KeyValuePair<string,object>>
    {
        Dictionary<string, object> _backingDict = new Dictionary<string, object>();
        string _fileName;
        bool _readOnly;
        public YamlFile(string fileName, bool autoInit = true, bool readOnly = false)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            _fileName = fileName;
            _readOnly = readOnly;
            if (autoInit)
                Read();
        }

        public void Read()
        {
            using (var x = new FileStream(_fileName, FileMode.Open))
            {
                var des = new Deserializer();
                var dict = des.Deserialize<Dictionary<string,object>>(new StreamReader(x));
                foreach(var entry in dict)
                {
                    this[entry.Key] = entry.Value;
                }
            }


        }
        public void Write()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(string key)
        {
            return Convert.ToBoolean(this[key]);
        }

        public Int32 GetInt32(string key)
        {
            return Convert.ToInt32(this[key]);
        }

        public double GetDouble(string key)
        {
            return Convert.ToDouble(this[key]);
        }

        public string GetString(string key)
        {
            return (string)this[key];
        }

        public object this[string key]
        {
            get { return _backingDict[key]; }
            set { if (_readOnly) throw new InvalidOperationException("Cannot change a value because it's set to read-only"); else _backingDict[key] = value; }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _backingDict.Keys;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result= this[binder.Name];
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _backingDict[binder.Name] = value;
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _backingDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _backingDict.GetEnumerator();
        }
    }
}
