﻿namespace FinalesFunkeln.Controls
{
    class PropertyObjectTreeViewItem:PacketTreeViewItem
    {
        private readonly bool _isPath;
        public string ClassName { get; private set; }
        public override string Header { get { return Property + (string.IsNullOrEmpty(ClassName) ? "(<dynamic>)" : "(" + ClassName + ")"); } }
        public override bool IncludeInPathLookup { get { return _isPath; } }

        public object Element { get; }

        public PropertyObjectTreeViewItem(PacketTreeViewItem parent, string property, string classname, object element, bool includeInPathLookup):base(parent,property)
        {
            ClassName = classname;
            _isPath = includeInPathLookup;
            Element = element;
        }
    }
}
