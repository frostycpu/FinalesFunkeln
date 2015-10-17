using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    public partial class ExtensionsList 
    {
        public ExtensionsList()
        {
            InitializeComponent();
        }

        public void AddExtension(IExtension extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            ExtensionsLb.Items.Add(extension);
        }
        public void AddExtensions(IEnumerable<IExtension> extensions)
        {
            if (extensions == null) throw new ArgumentNullException("extensions");
            foreach (var x in extensions)
                AddExtension(x);
        }

        public void Clear()
        {
            ExtensionsLb.Items.Clear();
        }

        private void ExtensionsLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IExtension item;
            if (e.AddedItems.Count < 1 || (item=e.AddedItems[0] as IExtension) == null) return;
            NameLabel.Text = item.Name;
            if (item is IInternalExtension)
            {
                NameLabel.FontStyle = FontStyles.Oblique;
                NameLabel.FontWeight = FontWeights.Bold;
            }
            else
            {
                NameLabel.FontStyle = FontStyles.Normal;
                NameLabel.FontWeight = FontWeights.Normal;
            }
            VersionLabel.Text = item.Version;
            AuthorLabel.Text = string.IsNullOrEmpty(item.Author) ? "Unknown" : item.Author;
            DescriptionTb.Text = !string.IsNullOrEmpty(item.Description)?item.Description:"No description available for this extension.\nThe creator was probably too lazy to add one.\n¯\\_(ツ)_/¯";
        }
    }
}
