using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FinalesFunkeln.Extensibility
{
    public interface IExtension
    {
        string Name { get; }
        string InternalName { get; }
        ExtensionVersion Version { get; }
        string Author { get; }
        string Description { get; }
        void Init(ExtensionManager extensionManager, UiManager uiManager, Dispatcher dispatcher);

        void ExtensionMessageReceived(IExtension sender, string command, object[] arguments);

    }
}
