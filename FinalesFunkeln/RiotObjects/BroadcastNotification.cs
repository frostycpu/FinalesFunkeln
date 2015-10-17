using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtmpSharp.IO;

namespace FinalesFunkeln.RiotObjects
{
    [Serializable]
    [SerializedName("com.riotgames.platform.broadcast.BroadcastNotification")]
    public class BroadcastNotification : ExternalizableJsonObject
    {
        public BroadcastNotification():base("com.riotgames.platform.broadcast.BroadcastNotification") { }
    }
}
