
using System;
using System.Windows;

namespace FinalesFunkeln.Extensibility.Ui
{
    public class View
    {
        public IExtension Owner { get; private set; }
        public string Name { get; private set; }
        public string InternalName { get; private set; }
        public UIElement Ui { get; private set; }
        //parameterless Constructor for serialization
        public View()
        {

        }
        public View(IExtension owner, string name, string internalName, UIElement ui)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if(internalName==null)
                throw new ArgumentNullException(nameof(internalName));
            if(ui==null)
                throw new ArgumentNullException(nameof(ui));
            Name = name;
            InternalName = internalName;
            Ui = ui;

        }
    }
}