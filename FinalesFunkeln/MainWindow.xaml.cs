using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using FinalesFunkeln.IO;
using RtmpSharp.Messaging;
using System.ComponentModel;
using FinalesFunkeln.Extensibility;
using FinalesFunkeln.Extensibility.Events;
using FinalesFunkeln.Lol;
using System.Globalization;
using System.Management;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CBF;
using FinalesFunkeln.Controls;
using FinalesFunkeln.Extensibility.Internal;
using FinalesFunkeln.Extensibility.Ui;
using FinalesFunkeln.Util;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace FinalesFunkeln
{
    partial class MainWindow
    {
        static readonly Random AppRandom = new Random();
        readonly RiotSerializationContext _serializationContext = RiotSerializationContext.Instance;

        const string ExtensionsDir = "extensions/";
        const string ExtensionsDependenciesDir = "extdependencies/";
        const string ConfigDir = "config/";
        const string InternalConfigDir = "config/internal/";
        const string LayoutConfigFilename = "Layout.cbf";
        const string DefaultLayoutDefinition = "FinalesFunkeln.Resources.Config.Layout.cbf";
        const string LolPropertiesFilename = "lol.properties";
        const string CertFileName = "data/certs/{0}.p12";
        const int RtmpPort = 2099;


        ProcessInjector _processInjector;
        LolProxy _proxy;

        string _rtmpAddress;
        X509Certificate2 _certificate;
        ExtensionManager _extensionManager;
        PropertiesFile _lolProperties;
        Process _lolClientProcess;
        LolClient _lolClient;

        //UI
        UiManager _uiManager;
        LayoutDocumentPane _mainPane;
        WindowInfo _savedWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            if (!Directory.Exists("data") || !File.Exists("sqlite3.dll"))
            {
                MessageBox.Show(Debugger.IsAttached ? @"""data"" folder and/or sqlite3.dll not found. Make sure to copy the data folder and sqlite3.dll to the output directory." : "Some files are missing, please reinstall.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uiManager = new UiManager();

            LoadExtensionDependencies();

            List<Assembly> assemblies = LoadExtensions();
            _extensionManager = new ExtensionManager(_uiManager, Dispatcher, ConfigDir, new FinalesFunkelnExtension());
            _extensionManager.AddAssemblies(assemblies);
            _extensionManager.InitExtensions();

            foreach (var x in _extensionManager.Extensions)
            {
                var item = new ExtensionMenuItem(x.Value);
                item.Views.AddRange(_uiManager.GetViewsByExtension(x.Value));
                ViewsMenuItem.Items.Add(item);
            }

            DockingManager.Layout.RootPanel = new LayoutPanel(_mainPane = new LayoutDocumentPane());
            

            foreach (var view in _uiManager.GetViews())
            {
                LayoutContent c = new DocumentViewControl(view);
                _mainPane.Children.Add(c);
            }

#if AIRDEBUG && DEBUG
            _processInjector = new ProcessInjector("adl");//AirDebugLauncher
#elif LCU
            _processInjector = new ProcessInjector("LeagueClient");//New LoL client
#else
            _processInjector = new ProcessInjector("lolclient");
#endif

            _processInjector.Injected += pi_Injected;
            _processInjector.ProcessFound += ProcessInjector_ProcessFound;
            _processInjector.ProcessExited += _processInjector_ProcessExited;
            _processInjector.Start();

        }

        private void LoadExtensionDependencies()
        {
            if (!Directory.Exists(ExtensionsDependenciesDir))
                Directory.CreateDirectory(ExtensionsDependenciesDir);
            
            foreach (var file in Directory.GetFiles(ExtensionsDependenciesDir, "*.dll"))
            {
                try
                {
                    Assembly.LoadFrom(file);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void _processInjector_ProcessExited(object sender, Process e)
        {
            _proxy.Close();
            _extensionManager.FireLolClientClosedEvent();
        }

        void LoadUiLayout()
        {
            string filename = Path.Combine(InternalConfigDir, LayoutConfigFilename);
            if (File.Exists(filename))
            {
                _savedWindow = Cbf.ReadFile<WindowInfo>(filename);
            }
            else
            {
                var assembly = Assembly.GetExecutingAssembly();

                using (var stream = assembly.GetManifestResourceStream(DefaultLayoutDefinition))
                {
                    _savedWindow = Cbf.Read<WindowInfo>(stream);
                }
            }
            _savedWindow.Convert(this, _uiManager);


        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            /*
                Loading the layout here causes the window to flash at its original position
                for a fraction of a second but a bug prevents us from loading 
                the layout earlier 
            */
            LoadUiLayout();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string date = "[" + DateTime.Now + "]";
            string error = ex == null ? date + "\n" + e.ExceptionObject : date + "\n" + ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace + "\n";
            File.AppendAllText("Error.log", error);

            if (!e.IsTerminating)
            {
                _extensionManager.ConsoleWriteLine("<<<Unhandled Exception>>>");
                _extensionManager.ConsoleWriteLine(ex);
                _extensionManager.ConsoleWriteLine("<<<Unhandled Exception End>>>");
            }

        }

        private List<Assembly> LoadExtensions()
        {
            if (!Directory.Exists(ExtensionsDir))
                Directory.CreateDirectory(ExtensionsDir);

            List<Assembly> ret = new List<Assembly>();
            foreach (var file in Directory.GetFiles(ExtensionsDir, "*.dll"))
            {
                try
                {
                    ret.Add(Assembly.LoadFrom(file));
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            return ret;
        }

        void ProcessInjector_ProcessFound(object sender, Process e)
        {
            ProcessInjector pi = sender as ProcessInjector;
            if (pi == null) return;

#if !LCU    //The new lol client has a separate process for the UI
            //sometimes it takes a while for the main module to be loaded...
            while (e.MainWindowHandle == IntPtr.Zero)
#endif
            Thread.Sleep(1000);
            string loldir = null;
            try
            {
#if AIRDEBUG && DEBUG
                string wmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", "adl.exe");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
                ManagementObjectCollection retObjectCollection = searcher.Get();
                foreach (ManagementObject retObject in retObjectCollection)
                    loldir = ProcessHelper.SplitCommandLineArgs((string)retObject["CommandLine"])[2];
#elif LCU
                loldir = Path.GetDirectoryName(e.MainModule.FileName) ?? string.Empty;
                loldir = Path.Combine(loldir, @"../../../../lol_air_client/releases/0.0.4.53/deploy");
#else
                loldir = Path.GetDirectoryName(e.MainModule.FileName) ?? string.Empty;
#endif
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Cannot access the lolclient process. If this error persists try runnig as an administrator.","Error");
                return;

            }
            _lolProperties = new PropertiesFile(Path.Combine(loldir, LolPropertiesFilename));
            var host = _lolProperties["host"];
            _rtmpAddress = host.Contains(",") ? host.Substring(0, host.IndexOf(',')) : host;

            if (_rtmpAddress == null) return;

            _certificate = GetCertificate(_rtmpAddress);

            if (_certificate == null)
            {
                Dispatcher.Invoke(() => MessageBox.Show(this, "This program is not compatible with your region: " + _lolProperties["platformId"] + ".\n", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));

                return;
            }

            if (!IsInstalled(_certificate))
            {
                MessageBoxResult res = MessageBoxResult.None;

                Dispatcher.Invoke(() => res = MessageBox.Show(this, "A certificate needs to be installed.\n\n" +
                                                "You can see which certificates are installed under Views->FinalesFunkeln->Certificates and uninstall them at any time.\n\nInstall the certificate now?", "Confirmation",
                                                MessageBoxButton.YesNo, MessageBoxImage.Information));
                if (res != MessageBoxResult.Yes)
                    return;
                InstallCertificate(_certificate);

            }

            InitProxy();
            _lolClient = new LolClient(loldir, _lolProperties, _proxy);
            _extensionManager.FireLolClientInjectedEvent(_lolClient);
            try
            {
                pi.Inject();
                _lolClientProcess = e;
            }
            catch (WarningException ex)
            {
                //I think this only happens when we try to inject into an already redirected app.
            }
            catch (AccessViolationException ex)
            {
                //Not sure what to do if that happens
                if(Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        void InitProxy()
        {
            if (_certificate == null)
                return;

            _proxy = new LolProxy(new IPEndPoint(IPAddress.Loopback, RtmpPort), new Uri(string.Format("rtmps://{0}:{1}", _rtmpAddress, RtmpPort)), _serializationContext, _certificate);

            _proxy.AcknowledgeMessageReceived += OnAckMessageReceived;
            _proxy.AsyncMessageReceived += OnAsyncMessageReceived;
            _proxy.ErrorMessageReceived += OnErrorMessageReceived;
            _proxy.RemotingMessageReceived += OnRemotingMessageReceived;
            _proxy.Disconnected += proxy_Disconnected;
            _proxy.Connected += proxy_Connected;
            _proxy.Listen();

        }

        void proxy_Connected(object sender, EventArgs e)
        {
            _extensionManager.FireLolClientConnectedEvent();
        }

        void proxy_Disconnected(object sender, EventArgs e)
        {
            _extensionManager.FireLolClientDisconnectedEvent();
        }

        void OnRemotingMessageReceived(object sender, RemotingMessageReceivedEventArgs args)
        {
            RemoteProcedureCallEventArgs rpc = new RemoteProcedureCallEventArgs(args.Destination, args.Operation, (dynamic)args.Message.Body);
            _extensionManager.FireRemoteProcedureCallEvent(rpc);
            args.Message.Body = rpc.Parameters;
        }


        void OnErrorMessageReceived(object sender, RemotingMessageReceivedEventArgs args)
        {
            RemoteProcedureCallResponseEventArgs rpc = new RemoteProcedureCallResponseEventArgs(args.Destination, args.Operation, (dynamic[])args.Message.Body, args.Error == null ? null : args.Error.RootCause);
            _extensionManager.FireErrorMessageReceivedEvent(rpc);
            if (args.Error != null)
                args.Error.RootCause = rpc.ResponseBody;
        }

        void OnAsyncMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            AsyncMessageEventArgs rpcr = new AsyncMessageEventArgs(args.Message.Body);
            _extensionManager.FireAsyncMessageReceivedEvent(rpcr);
            //args.Result.Body = rpcr.Body;
            args.Message.Body = rpcr.Body;
        }

        void OnAckMessageReceived(object sender, RemotingMessageReceivedEventArgs args)
        {
            RemoteProcedureCallResponseEventArgs rpc = new RemoteProcedureCallResponseEventArgs(args.Destination, args.Operation, (dynamic)args.Message.Body, args.Result.Body);
            _extensionManager.FireAcknowledgeMessageReceivedEvent(rpc);
            args.Result.Body = rpc.ResponseBody;
        }

        void pi_Injected(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                Title = "Finales Funkeln - Injected [" + _lolProperties["platformId"] + "]";
            }));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

            var result = MessageBoxResult.Yes;
            if (!Debugger.IsAttached && _lolClientProcess != null && !_lolClientProcess.HasExited)
                result = MessageBox.Show(this, "You are about to close this program.\nYou'll disconnect from pvp.net and if you are currently in a\ngame you'll receive a queue dodge penalty!\n\n Are you sure you want to exit?", "Confirm exit", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                _extensionManager.FireShutdownEvent();
                SaveUiLayout();
                if (_lolClientProcess != null && !_lolClientProcess.HasExited)
                    _lolClientProcess.Kill();
            }
            else
                e.Cancel = true;
        }

        void SaveUiLayout()
        {
            WindowInfo vl = new WindowInfo();
            vl.Convert(this);

            if (!Directory.Exists(InternalConfigDir))
                Directory.CreateDirectory(InternalConfigDir);
            Cbf.WriteFile(Path.Combine(InternalConfigDir, LayoutConfigFilename), vl);

        }

        internal bool IsInstalled(X509Certificate2 cert)
        {
            if (cert == null)
                throw new ArgumentNullException("cert");
            if (cert.Thumbprint == null)
                throw new ArgumentException("cert.Thumbprint must not be null");
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            bool result= store.Certificates.Find(X509FindType.FindByThumbprint, cert.Thumbprint, true).Count > 0;

            store.Close();

            return result;
        }

        internal void InstallCertificate(X509Certificate2 cert)
        {
            if (cert == null)
                throw new ArgumentNullException("cert");
            if (cert.Thumbprint == null)
                throw new ArgumentException("cert.Thumbprint must not be null");
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.MaxAllowed);

            if (store.Certificates.Find(X509FindType.FindByThumbprint, cert.Thumbprint, true).Count == 0)
                store.Add(cert);

            store.Close();
        }

        internal X509Certificate2 GetCertificate(string hostName)
        {
            string file = string.Format(CertFileName, hostName);
            if (string.IsNullOrEmpty(hostName) || !File.Exists(file))
                return null;
            return new X509Certificate2(File.ReadAllBytes(file), "");
        }

        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = e.OriginalSource as MenuItem;
            var data = mi?.DataContext as View;
            //TODO probably inefficient needs to be rewritten eventually
            if (data != null)
            {
                List<LayoutDocumentPane> panes = GetPanes();
                bool alreadyActive=false;
                LayoutDocumentPane biggest = null;
                if (panes.Count > 0)
                {
                    biggest = panes[0];
                    double biggestSize = biggest.DockWidth.Value*biggest.DockHeight.Value;
                    foreach (var x in panes)
                    {
                        foreach (var y in x.Children)
                        {
                            if (y is DocumentViewControl)
                            {
                                if ((y as DocumentViewControl).View.InternalName == data.InternalName)
                                {
                                    DockingManager.ActiveContent = y;
                                    alreadyActive = true;
                                }
                            }
                            else
                                throw new NotSupportedException();
                        }
                        var size = x.DockWidth.Value*x.DockHeight.Value;
                        if (size > biggestSize)
                        {
                            biggestSize = size;
                            biggest = x;
                        }
                    }
                }
                else
                {
                    biggest=new LayoutDocumentPane();
                    DockingManager.Layout.RootPanel.Children.Add(biggest);
                }
                if (!alreadyActive)
                {
                    var ctrl = new DocumentViewControl(data);
                    biggest.Children.Add(ctrl);
                    DockingManager.ActiveContent = ctrl;
                }
            }
        }

        List<LayoutDocumentPane> GetPanes()
        {
            List<LayoutDocumentPane> panes=new List<LayoutDocumentPane>();
            GetPanes(panes, DockingManager.Layout.Children);
            return panes;
        }

        void GetPanes(List<LayoutDocumentPane> panes, IEnumerable<ILayoutElement> elements)
        {
            foreach (var x in elements)
            {
                if (x is LayoutDocumentPane)
                    panes.Add(x as LayoutDocumentPane);
                else if (x is ILayoutContainer)
                    GetPanes(panes, (x as ILayoutContainer).Children);

            }
        }
    }
}
