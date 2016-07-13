using System;

namespace FinalesFunkeln.Extensibility.Internal.Controls
{
    internal abstract class PacketListItem
    {
        protected string Time{get; set; }
        
        internal PacketListItem()
        {
            Time = $"{DateTime.Now:HH:mm:ss}";
        }
    }
}
