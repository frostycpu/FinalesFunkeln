using System;
using FinalesFunkeln.Extensibility.Ui;
using Xceed.Wpf.AvalonDock.Layout;

namespace FinalesFunkeln.Controls
{
    [Serializable]
    public class DocumentViewControl:LayoutDocument
    {
        public View View { get; private set; }
        //parameterless Constructor for serialization
        public DocumentViewControl()
        {
            
        }
        public DocumentViewControl(View v)
        {
            View = v;
            Title = v.Name;
            Content = v.Ui;
        }

    }
}
