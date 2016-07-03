using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using FinalesFunkeln.Extensibility.Events;
using FinalesFunkeln.Extensibility.Internal.Controls;
using FinalesFunkeln.Extensibility.Ui;
using FinalesFunkeln.Lol;

namespace FinalesFunkeln.Extensibility.Internal
{
    internal class FinalesFunkelnExtension : IInternalExtension
    {
        public string Name => "FinalesFunkeln";
        public string InternalName => "frostycpu:FinalesFunkeln";
        public ExtensionVersion Version => "1.0.0.0";
        public string Author => "frostycpu";
        public string Description => "The application itself.";

        private readonly RichTextBox _textBox = new RichTextBox {IsReadOnly = true,};
        private readonly PacketOverviewUi _packetUi = new PacketOverviewUi();
        private readonly ClientStatus _clientStatus = new ClientStatus();
        private readonly CertificateList _certList = new CertificateList();
        private Dispatcher _dispatcher;

        public void Init(ExtensionManager extensionManager, UiManager uiManager, Dispatcher dispatcher)
        {
            uiManager.RegisterView(this, new View(this, "Output", "FinalesFunkeln:Output", _textBox));
            uiManager.RegisterView(this, new View(this, "Packets", "FinalesFunkeln:PacketOverview", _packetUi));
            uiManager.RegisterView(this, new View(this, "Status", "FinalesFunkeln:ClientStatus", _clientStatus));
            uiManager.RegisterView(this, new View(this, "Certificates", "FinalesFunkeln:Certificates", _certList));
            _dispatcher = dispatcher;

            extensionManager.AcknowledgeMessageReceived += pm_AcknowledgeMessageReceived;
            extensionManager.AsyncMessageReceived += pm_AsyncMessageReceived;
            extensionManager.ErrorMessageReceived += pm_ErrorMessageReceived;
            extensionManager.LolClientInjected += pm_LolClientInjected;
            extensionManager.LolClientConnected += pm_LolClientConnected;
            extensionManager.LolClientDisconnected += pm_LolClientDisconnected;
            extensionManager.LolClientClosed += pm_LolClientClosed;

        }

        private void pm_ErrorMessageReceived(object sender, RemoteProcedureCallResponseEventArgs e)
        {
            _dispatcher.InvokeAsync(() => _packetUi.PacketListBox.Items.Add(new ErrorListItem(e.Destination, e.Operation, e.Parameters, e.ResponseBody,e.InvokedByProxy)));
        }

        private void pm_AsyncMessageReceived(object sender, AsyncMessageEventArgs e)
        {
            _dispatcher.InvokeAsync(() => _packetUi.PacketListBox.Items.Add(new AsyncListItem(e.Body)));
        }

        private void pm_AcknowledgeMessageReceived(object sender, RemoteProcedureCallResponseEventArgs e)
        {
            _dispatcher.InvokeAsync(() => _packetUi.PacketListBox.Items.Add(new AcknowledgeListItem(e.Destination, e.Operation, e.Parameters, e.ResponseBody, e.InvokedByProxy)));
        }

        private void pm_LolClientClosed(object sender, EventArgs e)
        {
            _dispatcher.InvokeAsync(() => _clientStatus.Status = ClientStates.Waiting);
        }

        private void pm_LolClientConnected(object sender, EventArgs args)
        {
            _dispatcher.InvokeAsync(() => _clientStatus.Status = ClientStates.Connected);
        }

        private void pm_LolClientDisconnected(object sender, EventArgs args)
        {
            _dispatcher.InvokeAsync(() => _clientStatus.Status = ClientStates.Injected);
        }

        private void pm_LolClientInjected(object sender, LolClient e)
        {
            _dispatcher.InvokeAsync(() => _clientStatus.Status = ClientStates.Injected);
        }

        public void ExtensionMessageReceived(IExtension sender, string command, object[] arguments)
        {
            switch (command)
            {
                case "WriteLine":
                    if (arguments.Length > 0)
                        WriteLine(arguments[0]);
                    break;
            }
        }

        public void WriteLine(object obj)
        {
            if (obj != null)
                _textBox.AppendText(obj.ToString());
            _textBox.AppendText(Environment.NewLine);
            _textBox.ScrollToEnd();
        }
    }
}
