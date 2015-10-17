using System.Windows.Controls;
using System.Windows.Media;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    public partial class ClientStatus
    {
        const string WaitingInfo = "Start League of Legends now.";
        const string InjectedInfo = "The application has successfully injected itself.";
        const string ConnectedInfo = "Everything went well. We are connected to the League of Legends servers.";

        ClientStates _status;
        public ClientStates Status
        {
            get { return _status; }
            set 
            {
                ChangeStatus(value);
                _status = value;
            }
        }
        public ClientStatus()
        {
            InitializeComponent();
            Status = ClientStates.Waiting;
        }

        void ChangeStatus(ClientStates status)
        {
            switch (status)
            {
                case ClientStates.Waiting:
                    StatusText.Text = "Waiting...";
                    StatusIcon.Fill = (Brush)Resources["RedBrush"];
                    InfoText.Text = WaitingInfo;
                    break;
                case ClientStates.Injected:
                    StatusText.Text = "Injected";
                    StatusIcon.Fill = (Brush)Resources["YellowBrush"];
                    InfoText.Text = InjectedInfo;
                    break;
                case ClientStates.Connected:
                    StatusText.Text = "Connected";
                    StatusIcon.Fill = (Brush)Resources["GreenBrush"];
                    InfoText.Text = ConnectedInfo;
                    break;
            }
        }
    }

    public enum ClientStates
    {
        Waiting,
        Injected,
        Connected
    }
}
