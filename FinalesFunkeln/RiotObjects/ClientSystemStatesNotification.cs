using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtmpSharp.IO;

namespace FinalesFunkeln.RiotObjects
{
    [Serializable]
    //[SerializedName("com.riotgames.platform.systemstate.ClientSystemStatesNotification")]
    public class ClientSystemStatesNotification : ExternalizableJsonObject
    {
        public ClientSystemStatesNotification():base("com.riotgames.platform.systemstate.ClientSystemStatesNotification") { }
    }
}
