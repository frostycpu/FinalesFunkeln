using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.IO
{
    public class PropertiesFile:Dictionary<string,string>
    {
        string _fileName;
        bool _readOnly;
        public PropertiesFile(string fileName, bool autoInit=true, bool readOnly=false)
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
            string[] lines = File.ReadAllLines(_fileName);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("#"))
                    continue;
                string[] parts = line.Split('=');
                Add(parts[0], string.Join("=", parts.Skip(1)));
            }
        }
        public void Write()
        {
            if (_readOnly)
                throw new InvalidOperationException(_fileName+ " is opened as read only");
            StringBuilder sb = new StringBuilder();
            foreach(var kv in this)
            {
                sb.Append(kv.Key).Append('=').AppendLine(kv.Value);
            }
            File.WriteAllText(_fileName, sb.ToString());
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
            return this[key];
        }

        public new string this[string key] 
        {
            get { return base[key]; }
            set { if (_readOnly)throw new InvalidOperationException("Cannot change a value because it's set to read-only"); else base[key] = value; }
        }
    }
}
