using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FinalesFunkeln.Controls
{
    abstract class PacketTreeViewItem:INotifyPropertyChanged
    {
        private bool _expanded;
        private readonly ObservableCollection<PacketTreeViewItem> _items = new ObservableCollection<PacketTreeViewItem>();

        public ObservableCollection<PacketTreeViewItem> Items { get { return _items; } }
        public PacketTreeViewItem Parent { get; private set; }
        public string Property { get; private set; }
        public abstract string Header { get; }
        public abstract bool IncludeInPathLookup { get; }
        public bool IsNodeExpanded { get { return _expanded; } set
        {
            if(value!=_expanded)
            {
                _expanded = value;
                NotifyPropertyChanged();
            }
        } }

        public event PropertyChangedEventHandler PropertyChanged;
        public PacketTreeViewItem(PacketTreeViewItem parent, string property)
        {
            Parent = parent;
            Property = property;
        }

        void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            if(PropertyChanged!=null)
                PropertyChanged(this,new PropertyChangedEventArgs(name));
        }

        public string GetPath()
        {
            StringBuilder sb=new StringBuilder();
            var list = new LinkedList<string>();
            var i = this;
            while (i != null)
            {
                if (!i.IncludeInPathLookup)
                    break;
                list.AddFirst(i.Property);
                i = i.Parent;
            }
            bool isFirst = true;
            foreach (var x in list)
            {
                if (!isFirst)
                {
                    sb.Append('.');
                }
                sb.Append(x);
                isFirst = false;
            }
            return sb.ToString();
        }
    }
}
