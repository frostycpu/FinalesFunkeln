using System;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal class PacketListItem
    {
        protected string Time{get; set; }

        internal PacketListItem()
        {
            Time = string.Format("{0:HH:mm:ss}",DateTime.Now);
        }
    }
}
