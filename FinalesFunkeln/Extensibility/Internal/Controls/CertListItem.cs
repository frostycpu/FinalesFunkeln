using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class CertListItem:INotifyPropertyChanged
    {
        readonly X509Certificate2 _cert;
        bool _installed;
        string _name;
        public string Header { get { return _name; } set { if (value != _name) { _name = value; NotifyPropertyChanged(); } } }
        public bool Installed { get { return _installed; } set { if (value != _installed) { _installed = value; NotifyPropertyChanged(); } } }
        public X509Certificate2 Certificate { get { return _cert; } }
        internal CertListItem(string name, X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");

            _cert = certificate;
            _name = name ?? certificate.Subject;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged([CallerMemberName] string name="")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
