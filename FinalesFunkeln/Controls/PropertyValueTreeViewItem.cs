namespace FinalesFunkeln.Controls
{
    class PropertyValueTreeViewItem : PacketTreeViewItem
    {
        private readonly bool _isPath;
        public string Value { get; private set; }
        public override string Header { get { return Property + " = " + Value; } }
        public override bool IncludeInPathLookup { get { return _isPath; } }

        public PropertyValueTreeViewItem(PacketTreeViewItem parent, string property, string value, bool includeInPathLookup):base(parent,property)
        {
            Value = value;
            _isPath = includeInPathLookup;
        }
    }
}
