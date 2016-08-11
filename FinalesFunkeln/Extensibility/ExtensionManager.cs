using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using FinalesFunkeln.Extensibility.Events;
using FinalesFunkeln.Extensibility.Internal;
using FinalesFunkeln.Lol;

namespace FinalesFunkeln.Extensibility
{
    public class ExtensionManager
    {
        readonly Dictionary<string, IExtension> _extensions;
        int _internalExtensionsCount;
        readonly Dispatcher _dispatcher;
        readonly UiManager _uiManager;

        public event EventHandler<RemoteProcedureCallEventArgs> RemoteProcedureCall;
        public event EventHandler<RemoteProcedureCallResponseEventArgs> AcknowledgeMessageReceived;
        public event EventHandler<RemoteProcedureCallResponseEventArgs> ErrorMessageReceived;
        public event EventHandler<AsyncMessageEventArgs> AsyncMessageReceived;
        public event EventHandler<LolClient> LolClientInjected;
        public event EventHandler LolClientConnected;
        public event EventHandler LolClientDisconnected;
        public event EventHandler LolClientClosed;
        public event EventHandler Shutdown;

        internal Dictionary<string,IExtension> Extensions { get { return _extensions; } }

        internal ExtensionManager(UiManager uiManager, Dispatcher dispatcher, string configDirectory,params IInternalExtension[] internalExtensions)
        {
            if (uiManager == null)
                throw new ArgumentNullException("uiManager");
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            _internalExtensionsCount = internalExtensions.Length;
            
            _dispatcher = dispatcher;
            _uiManager = uiManager;

            _extensions = new Dictionary<string, IExtension>();
            foreach (var p in internalExtensions)
                _extensions.Add(p.InternalName, p);
        }

        internal void FireRemoteProcedureCallEvent(RemoteProcedureCallEventArgs args)
        {
            if (RemoteProcedureCall != null)
                RemoteProcedureCall(this, args);
        }

        internal void FireAcknowledgeMessageReceivedEvent(RemoteProcedureCallResponseEventArgs args)
        {
            if (AcknowledgeMessageReceived != null)
                AcknowledgeMessageReceived(this, args);
        }

        internal void FireErrorMessageReceivedEvent(RemoteProcedureCallResponseEventArgs args)
        {
            if (ErrorMessageReceived != null)
                ErrorMessageReceived(this, args);
        }

        internal void FireAsyncMessageReceivedEvent(AsyncMessageEventArgs args)
        {
            if (AsyncMessageReceived != null)
                AsyncMessageReceived(this, args);
        }

        internal void FireLolClientInjectedEvent(LolClient client)
        {
            if (LolClientInjected != null)
                LolClientInjected(this, client);
        }

        internal void FireLolClientConnectedEvent()
        {
            if (LolClientConnected != null)
                LolClientConnected(this, new EventArgs());
        }

        internal void FireLolClientDisconnectedEvent()
        {
            if (LolClientDisconnected != null)
                LolClientDisconnected(this, new EventArgs());
        }

        internal void FireLolClientClosedEvent()
        {
            if (LolClientClosed != null)
                LolClientClosed(this, new EventArgs());
        }

        internal void FireShutdownEvent()
        {
            if (Shutdown != null)
                Shutdown(this, new EventArgs());
        }

        internal void InitExtensions()
        {
            foreach (var p in _extensions)
            {
                try
                {
                    p.Value.Init(this, _uiManager, _dispatcher);
                    ConsoleWriteLine(string.Format("[{0}:{1}] Initialized",p.Value.Name,p.Value.Version));
                }
                catch (Exception ex)
                {
                    ConsoleWriteLine(string.Format("An error occured while initializing extension <{0}>",p.Value.Name));
                    ConsoleWriteLine(ex);
                }
            }

            ConsoleWriteLine("All Extensions initialized!");
            ConsoleWriteLine(string.Format("\tInternal: {0}", _internalExtensionsCount));
            ConsoleWriteLine(string.Format("\tExternal: {0}", _extensions.Count - _internalExtensionsCount));
            ConsoleWriteLine(string.Format("\tTotal: {0} ", _extensions.Count));
        }


        internal void AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var asm in assemblies)
            {
                Type[] types = asm.GetTypes();
                foreach(var type in types)
                {
                    ConstructorInfo constructor;
                    if (type.GetInterfaces().Contains(typeof(IExtension)) && type.IsClass && !type.IsAbstract && (constructor = type.GetConstructor(Type.EmptyTypes)) != null)
                    {
                        IExtension p = (IExtension)constructor.Invoke(null);
                        _extensions.Add(p.InternalName,p);
                        break;
                    }
                }
            }
        }

        public void SendMessage(IExtension sender, string receiver, string command, params object[] arguments)
        {
            if(sender==null)
                throw new ArgumentNullException(nameof(sender));
            if (_extensions.ContainsKey(receiver))
            {
                _extensions[receiver].ExtensionMessageReceived(sender,command,arguments);
            }
        }

        internal void SendMessage(string receiver, string command, params object[] arguments)
        {
            if (_extensions.ContainsKey(receiver))
            {
                _extensions[receiver].ExtensionMessageReceived(null,command,arguments);
            }
        }

        public void ConsoleWriteLine(IExtension sender, object obj)
        {
            SendMessage(sender, "frostycpu:FinalesFunkeln", "WriteLine", obj);
        }

        internal void ConsoleWriteLine(object obj)
        {
            SendMessage("frostycpu:FinalesFunkeln", "WriteLine", obj);
        }
    }
}
