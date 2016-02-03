namespace FinalesFunkeln.Controls
{
    class RootTreeViewItem:PacketTreeViewItem
    {
        public override string Header { get { return _name; } }
        public override bool IncludeInPathLookup { get { return false; } }
        private readonly string _name;

        public RootTreeViewItem(string name):base(null,name)
        {
            _name = name;
        }
    }
}
