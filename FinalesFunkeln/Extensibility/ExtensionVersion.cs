using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility
{
    public struct ExtensionVersion
    {
        private uint[] _version;
        public uint[] Version { get { return _version; } private set { _version = value; } }

        public ExtensionVersion(uint[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                throw new ArgumentException("<bytes> cannot be null and must have a length of 4");
            _version=new uint[4];
            Array.Copy(bytes, _version,4);
        }

        public ExtensionVersion(uint a, uint b, uint c, uint d) : this(new[] { a, b, c, d }) { }

        public ExtensionVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || !version.Contains('.'))
                throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
            string[] parts = version.Split('.');
            if (parts.Length != 4)
                throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
            _version = new uint[4];
            for(int i=0;i<4;i++)
            {
                uint b;
                if (!uint.TryParse(parts[i], out b))
                    throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
                _version[i] = b;
            }
        }

        public override string ToString()
        {
            return _version[0] + "." + _version[1] + "." + _version[2] + "." + _version[3];
        }
        
        public static implicit operator ExtensionVersion(string version)
        {
            return new ExtensionVersion(version);
        }

        public static implicit operator ExtensionVersion(uint[] version)
        {
            return new ExtensionVersion(version);
        }

        public static implicit operator string(ExtensionVersion version)
        {
            return version.ToString();
        }
    }
}
