using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.Extensibility.Events;
using FinalesFunkeln.Extensibility.Ui;

namespace FinalesFunkeln.Extensibility
{
    public class UiManager
    {
        readonly Dictionary<string, View> _viewsByInternalName  = new Dictionary<string, View>();
        readonly Dictionary<IExtension,List<View>> _viewsByExtension  =new Dictionary<IExtension, List<View>>();
        internal event EventHandler<ViewRegisteredEventArgs> ViewRegistered;

        internal UiManager()
        {
        }

        public void RegisterView(IExtension owner, View view)
        {
            if(owner==null)
                throw new ArgumentNullException(nameof(owner));
            if(view==null)
                throw new ArgumentNullException(nameof(view));
            if(_viewsByInternalName.ContainsKey(view.InternalName))
                throw new ArgumentException($"A View with name <{view.InternalName}> is already registered. Make sure Views have a unique name.");
            _viewsByInternalName.Add(view.InternalName,view);
            if (_viewsByExtension.ContainsKey(owner))
            {
                _viewsByExtension[owner].Add(view);
            }
            else
            {
                var views = new List<View> {view};
                _viewsByExtension.Add(owner, views);
            }

            ViewRegistered?.Invoke(this, new ViewRegisteredEventArgs(owner,view));
        }

        public void RegisterViews(IExtension owner, params View[] views)
        {
            foreach (var v in views)
            {
                RegisterView(owner, v);
            }
        }

        internal List<View> GetViewsByExtension(IExtension extension)
        {
            return !_viewsByExtension.ContainsKey(extension) ? new List<View>(0) : _viewsByExtension[extension];
        }

        internal IEnumerable<View> GetViews()
        {
            foreach (var x in _viewsByInternalName)
                yield return x.Value;
        }

        internal bool HasView(string internalName)
        {
            return _viewsByInternalName.ContainsKey(internalName);
        }

        internal View GetView(string internalName)
        {
            return _viewsByInternalName[internalName];
        }
    }
}
