using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FinalesFunkeln.Controls.Attributes;
using FinalesFunkeln.IO;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace FinalesFunkeln.Controls
{
    public partial class ObjectTree : UserControl
    {
        public ObjectTree()
        {
            InitializeComponent();
            
        }

        public void SetRoot(string rootname, object obj)
        {
            ObjectTreeView.Items.Clear();
            PacketTreeViewItem root = BuildObjectTree(null, obj, rootname, true, false);
            root.IsNodeExpanded = true;

            ObjectTreeView.Items.Add(root);

        }

        private RootTreeViewItem BuildAckTree(string destination, string operation, object[] args, object response)
        {
            RootTreeViewItem root = new RootTreeViewItem("Invoke");

            root.Items.Add(new PropertyValueTreeViewItem(root, "Destination", destination, false));
            root.Items.Add(new PropertyValueTreeViewItem(root, "Operation", operation, false));
            BuildObjectTree(root, args, "Arguments", true, false);
            BuildObjectTree(root, response, "Return", true, false);
            return root;
        }

        private RootTreeViewItem BuildErrorTree(string destination, string operation, object[] args, object response)
        {
            RootTreeViewItem root = new RootTreeViewItem("Invoke");

            root.Items.Add(new PropertyValueTreeViewItem(root, "Destination", destination, false));
            root.Items.Add(new PropertyValueTreeViewItem(root, "Operation", operation, false));
            BuildObjectTree(root, args, "Arguments", true, false);
            BuildObjectTree(root, response, "Return", true, false);
            return root;
        }

        private RootTreeViewItem BuildAsyncTree(object body)
        {

            RootTreeViewItem root = new RootTreeViewItem("AsyncMessage");
            BuildObjectTree(root, body, "Body", true, false);
            return root;
        }

        private PacketTreeViewItem BuildObjectTree(PacketTreeViewItem parent, object obj, string name, bool expand = false, bool isPath = true)
        {

            PacketTreeViewItem tvi = null;

            if (obj == null)
            {
                tvi = new PropertyValueTreeViewItem(parent, name, "null", isPath);
            }
            else if (obj is string)
            {
                tvi = new PropertyValueTreeViewItem(parent, name, EscapeString(obj as string), isPath);
            }
            else if (obj.GetType().IsArray)
            {
                object[] arr = obj as object[];
                tvi = new PropertyObjectTreeViewItem(parent, name, "Array" + '[' + arr.Length.ToString() + "]", isPath);
                for (int i = 0; i < arr.Length; i++)
                {
                    BuildObjectTree(tvi, arr[i], '[' + i.ToString() + ']');
                }
            }
            else if (obj is ArrayCollection)
            {
                ArrayCollection col = obj as ArrayCollection;
                tvi = new PropertyObjectTreeViewItem(parent, name, "ArrayCollection" + '[' + col.Count.ToString() + "]", isPath);
                for (int i = 0; i < col.Count; i++)
                {
                    BuildObjectTree(tvi, col[i], '[' + i.ToString() + ']');
                }

            }
            else if (obj is ArrayList)
            {
                ArrayList col = obj as ArrayList;
                tvi = new PropertyObjectTreeViewItem(parent, name, "Array" + '[' + col.Count.ToString() + "]", isPath);
                for (int i = 0; i < col.Count; i++)
                {
                    BuildObjectTree(tvi, col[i], '[' + i.ToString() + ']');
                }

            }
            else if (obj is IDictionary<string, object>)
            {
                IDictionary<string, object> dict = obj as IDictionary<string, object>;
                if (obj is AsObject)
                {
                    tvi = new PropertyObjectTreeViewItem(parent, name, (obj as AsObject).TypeName, isPath);
                }
                else
                {
                    tvi = new PropertyObjectTreeViewItem(parent, name, RiotSerializationContext.Instance.GetAlias(obj.GetType().GetGenericTypeDefinition().FullName), isPath);
                }
                foreach (var kv in dict)
                {
                    BuildObjectTree(tvi, kv.Value, kv.Key);
                }
            }
            else if (obj is DateTime)
            {
                tvi = new PropertyValueTreeViewItem(parent, name, obj.ToString(), isPath);
            }
            else if (obj.GetType().Namespace == "System")
            {
                tvi = new PropertyValueTreeViewItem(parent, name, obj.ToString(), isPath);
            }
            else if (Attribute.IsDefined(obj.GetType(), typeof(SerializableAttribute)))
            {
                PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                tvi = new PropertyObjectTreeViewItem(parent, name, RiotSerializationContext.Instance.GetAlias(obj.GetType().FullName), isPath);
                foreach (var prop in pis)
                {
                    if (prop.CanRead && prop.GetIndexParameters().Length == 0 && !Attribute.IsDefined(prop, typeof(HiddenAttribute)))
                    {
                        string pname = null;
                        SerializedNameAttribute[] attributes = (SerializedNameAttribute[])prop.GetCustomAttributes(typeof(SerializedNameAttribute), false);
                        if (attributes.Length == 1)
                            pname = attributes[0].SerializedName;
                        else if (attributes.Length > 0)
                        {
                            foreach (var a in attributes)
                            {
                                if (a.Canonical)
                                    pname = a.SerializedName;
                            }
                            if (pname == null)
                                pname = attributes[0].SerializedName;
                        }
                        else
                            pname = prop.Name;
                        BuildObjectTree(tvi, prop.GetValue(obj), pname);
                    }
                }
            }
            if (tvi != null)
            {
                if(parent!=null)
                parent.Items.Add(tvi);
                tvi.IsNodeExpanded = expand;
            }
            return tvi;
        }

        private static string EscapeString(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        private void CopyPropName_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            var cm = mi.CommandParameter as ContextMenu;
            if (cm == null) return;
            var element = cm.PlacementTarget as FrameworkElement;
            if (element == null) return;
            var g = element.DataContext as PacketTreeViewItem;
            if (g != null)
                Clipboard.SetText(g.Property);
        }

        private void CopyProp_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            var cm = mi.CommandParameter as ContextMenu;
            if (cm == null) return;
            var element = cm.PlacementTarget as FrameworkElement;
            if (element == null) return;
            var g = element.DataContext as PropertyValueTreeViewItem;
            if (g != null)
                Clipboard.SetText(g.GetPath() + " = " + g.Value);
        }

        private void CopyPropValue_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            var cm = mi.CommandParameter as ContextMenu;
            if (cm == null) return;
            var element = cm.PlacementTarget as FrameworkElement;
            if (element == null) return;
            var g = element.DataContext as PropertyValueTreeViewItem;
            if (g != null)
                Clipboard.SetText(g.Value);
        }

        private void CopyPropPath_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            var cm = mi.CommandParameter as ContextMenu;
            if (cm == null) return;
            var element = cm.PlacementTarget as FrameworkElement;
            if (element == null) return;
            var g = element.DataContext as PacketTreeViewItem;
            if (g == null) return;
            Clipboard.SetText(g.GetPath());
        }

        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.C || (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control) return;
            var tv = sender as TreeViewItem;
            if (tv == null || ObjectTreeView.SelectedItem != tv.DataContext) return;
            var pv = tv.DataContext as PropertyValueTreeViewItem;
            Clipboard.SetText((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? (tv.DataContext as PacketTreeViewItem).GetPath() :
                (pv != null) ? pv.GetPath() + " = " + pv.Value : (tv.DataContext as PropertyObjectTreeViewItem).ClassName);
        }

        private void CopyClassName_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            var cm = mi.CommandParameter as ContextMenu;
            if (cm == null) return;
            var element = cm.PlacementTarget as FrameworkElement;
            if (element == null) return;
            var g = element.DataContext as PropertyObjectTreeViewItem;
            if (g == null) return;
            Clipboard.SetText(g.ClassName);
        }
        
    }
}
