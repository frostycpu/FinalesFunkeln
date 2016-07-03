using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FinalesFunkeln.Extensibility;
using FinalesFunkeln.IO;

namespace FinalesFunkeln.Lol
{
    public class LolClient
    {
        const string GameDataFile="assets/data/gameStats/gameStats_en_US.sqlite";

        public LolClientImages Images { get; private set; }

        public LolClientGameData GameData { get; private set; }

        public PropertiesFile Properties { get; private set; }
        public LolConnection Connection { get; private set; }

        public ExtensionManager ExtensionManager { get; }
        
        internal LolClient(string directory, PropertiesFile properties, LolProxy proxy, ExtensionManager extManager)
        {
            if (proxy == null)
                throw new ArgumentNullException("proxy");
            if(!Directory.Exists(directory))
                throw new ArgumentException("directory");
            GameData = new LolClientGameData(Path.Combine(directory, GameDataFile));
            Images = new LolClientImages(directory, GameData);
            Connection=new LolConnection(proxy, extManager);
            Properties = properties;
            ExtensionManager=extManager;
        }

    }
}
