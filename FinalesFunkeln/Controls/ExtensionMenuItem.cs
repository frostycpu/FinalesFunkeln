using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.Extensibility;
using FinalesFunkeln.Extensibility.Ui;

namespace FinalesFunkeln.Controls
{
    class ExtensionMenuItem
    {
        private readonly IExtension _extension;
        public List<View> Views { get; } = new List<View>();
        public string Name => _extension.Name;

        public ExtensionMenuItem(IExtension extension)
        {
            _extension = extension;
        }
    }
}
