using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    public partial class CertificateList 
    {
        public CertificateList()
        {
            InitializeComponent();
        }

        private void InstallAll_Click(object sender, RoutedEventArgs e)
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            foreach (var x in CertListBox.Items)
            {
                var ci = x as CertListItem;
                if (ci == null) continue;
                if (ci.Certificate.Thumbprint != null)
                {
                    if (store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count == 0)
                        store.Add(ci.Certificate);

                    ci.Installed = store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count > 0;
                }
            }
            store.Close();
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            var ci = CertListBox.SelectedItem as CertListItem;
            if (ci != null && ci.Certificate.Thumbprint != null)
            {
                if (store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count == 0)
                    store.Add(ci.Certificate);

                ci.Installed = store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count > 0;
            }
            store.Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            var ci = CertListBox.SelectedItem as CertListItem;
            if (ci != null && ci.Certificate.Thumbprint != null)
            {
                if (store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count != 0)
                    store.Remove(ci.Certificate);

                ci.Installed = store.Certificates.Find(X509FindType.FindByThumbprint, ci.Certificate.Thumbprint, true).Count > 0;
            }
            store.Close();
        }

        private void CertificateList_OnInitialized(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("data/certs/", "*.p12");

            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            foreach (var x in files)
            {
                var certificate = new X509Certificate2(File.ReadAllBytes(x), "");
                var li = new CertListItem(Path.GetFileNameWithoutExtension(x), certificate);
                if (certificate.Thumbprint != null)
                {
                    X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, true);
                    li.Installed = certs.Count > 0;
                    CertListBox.Items.Add(li);
                }
            }
            store.Close();
        }
    }
}
