using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FinalesFunkeln.Controls.Attributes;
using FinalesFunkeln.Extensibility.Internal.Controls;
using FinalesFunkeln.IO;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    public partial class PacketOverviewUi
    {
        public PacketOverviewUi()
        {
            InitializeComponent();
        }

        private void PacketListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || e.AddedItems[0] == null)
                return;

            object item = (sender as ListBox).SelectedItem;
            AcknowledgeListItem ack = item as AcknowledgeListItem;
            AsyncListItem async = item as AsyncListItem;
            ErrorListItem err = item as ErrorListItem;
            
            if (ack != null)
                ObjectTree.SetRoot("Invoke", BuildAck(ack.ServiceName, ack.Operation, ack.InvokeArguments, ack.Response));
            else if (async != null)
                ObjectTree.SetRoot("AsyncMessage", BuildAsync(async.Body));
            else if (err != null)
                ObjectTree.SetRoot("Invoke", BuildError(err.ServiceName, err.Operation, err.InvokeArguments, err.Error));

        }
        private AsObject BuildAck(string destination, string operation, object[] args, object response)
        {
            AsObject obj = new AsObject();

            obj.Add("Destination", destination);
            obj.Add("Operation", operation);
            obj.Add("Arguments", args);
            obj.Add("Return", response);
            return obj;
        }
        private AsObject BuildError(string destination, string operation, object[] args, object response)
        {
            AsObject obj = new AsObject();

            obj.Add("Destination", destination);
            obj.Add("Operation", operation);
            obj.Add("Arguments", args);
            obj.Add("Return", response);
            return obj;
        }
        private AsObject BuildAsync(object body)
        {
            AsObject obj = new AsObject();
            obj.Add("Body", body);
            return obj;
        }
        private void ClearPacketList_Click(object sender, RoutedEventArgs e)
        {
            PacketListBox.Items.Clear();
        }
    }
}
