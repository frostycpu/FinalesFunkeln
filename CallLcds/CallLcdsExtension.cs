using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using FinalesFunkeln.Extensibility;
using FinalesFunkeln.Extensibility.Ui;
using FinalesFunkeln.Lol;
using ICSharpCode.AvalonEdit;

namespace CallLcds
{
    public class CallLcdsExtension : IExtension
    {
        public string Author => "frostycpu";

        public string Description => "Send arbitrary RTMP-packets to the server and inspect the results.";

        public string InternalName => "frostycpu:CallLcds";

        public string Name => "CallLcds";

        public ExtensionVersion Version => System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();

        LolClient client;

        CallLcdsUi ui=new CallLcdsUi();
        public void ExtensionMessageReceived(IExtension sender, string command, object[] arguments)
        {
        }

        public void Init(ExtensionManager extensionManager, UiManager uiManager, System.Windows.Threading.Dispatcher dispatcher)
        {
            extensionManager.LolClientInjected += (sender, e) => client = e;
            extensionManager.LolClientConnected += (sender, e) => ui.Client = client;
            extensionManager.LolClientDisconnected += (sender, e) => ui.Client = null;
            extensionManager.LolClientClosed += (sender, e) => client = null;

            uiManager.RegisterView(this, new View(this, "Call Lcds", "CallLcds:Main", ui));
            
        }
        
    }
}
