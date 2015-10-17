using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Extensibility
{
    public struct ExtensionVersion
    {
        private uint _version;
        public uint Version { get { return _version; } private set { _version = value; } }
        public ExtensionVersion(uint version)
        {
            _version = version;
        }

        public ExtensionVersion(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                throw new ArgumentException("<bytes> cannot be null and must have a length of 4");
            _version =(uint)( bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3]);
        }

        public ExtensionVersion(byte a, byte b, byte c, byte d) : this(new[] { a, b, c, d }) { }

        public ExtensionVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || !version.Contains('.'))
                throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
            string[] parts = version.Split('.');
            if (parts.Length != 4)
                throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
            _version = 0;
            for(int i=0;i<4;i++)
            {
                byte b;
                if (!byte.TryParse(parts[i], out b))
                    throw new ArgumentException('<' + version + "> is not a valid version. The correct format is <X.X.X.X>");
                _version = (uint)(_version | b << ((3 - i)*8));
            }
        }

        public override string ToString()
        {
            return (Version >> 24) + "." + ((Version >> 16) & 0xFF) + "." + ((Version >> 8) & 0xFF) + "." + (Version & 0xFF);
        }

        public static implicit operator ExtensionVersion(uint version)
        {
            return new ExtensionVersion(version);
        }

        public static implicit operator ExtensionVersion(string version)
        {
            return new ExtensionVersion(version);
        }

        public static implicit operator ExtensionVersion(byte[] version)
        {
            return new ExtensionVersion(version);
        }

        public static implicit operator string(ExtensionVersion version)
        {
            return version.ToString();
        }
    }
}
