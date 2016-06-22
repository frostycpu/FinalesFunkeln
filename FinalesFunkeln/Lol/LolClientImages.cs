using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FinalesFunkeln.Lol
{
    public class LolClientImages
    {
        const string IconFile = "assets/images/champions/{0}_Square_0.png";
        const string RandomIconFile = "assets/images/champions/Random_0.jpg";
        const string PortraitFile = "assets/images/champions/{0}_{1}.jpg";
        const string SplashFile = "assets/images/champions/{0}_Splash_{1}.jpg";
        const string SummonerSpellFile = "data/lolimg/summonerspells/{0}.png";
        const string WardSkinFile = "assets/storeImages/content/ward_skins/wardskin_{0}.jpg";
        readonly string _clientDir;
        readonly LolClientGameData _gameData;

        readonly Dictionary<int, BitmapImage> iconCache = new Dictionary<int, BitmapImage>();
        readonly Dictionary<int, BitmapImage> splashCache = new Dictionary<int, BitmapImage>();
        readonly Dictionary<int, BitmapImage> portraitCache = new Dictionary<int, BitmapImage>();
        readonly Dictionary<int, BitmapImage> spellIconCache = new Dictionary<int, BitmapImage>();
        readonly Dictionary<int, BitmapImage> wardSkinCache = new Dictionary<int, BitmapImage>();

        internal LolClientImages(string basePath, LolClientGameData gameData)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");
            _clientDir = basePath;
            _gameData = gameData;
        }

        public BitmapImage GetChampionIcon(int championId)
        {
            string name = null;
            if (championId != 0)
                name = _gameData.GetChampionData(championId)?.Name;
            BitmapImage bi;
            if (!iconCache.ContainsKey(championId))
            {
                bi = new BitmapImage(new Uri(Path.Combine(_clientDir, name == null ? RandomIconFile : string.Format(IconFile, name))));
                iconCache[championId] = bi;
                bi.Freeze();
            }
            else
                bi = iconCache[championId];
            return bi;
        }

        public BitmapImage GetChampionPortrait(int championId, int skinIndex = 0)
        {
            int id = championId * 1000 + skinIndex;
            string name = null;
            if (championId != 0)
                name = _gameData.GetChampionData(championId)?.Name;
            BitmapImage bi;
            if (!portraitCache.ContainsKey(id))
            {
                bi = new BitmapImage(new Uri(Path.Combine(_clientDir, string.Format(PortraitFile, name, skinIndex))));
                portraitCache[id] = bi;
                bi.Freeze();
            }
            else
                bi = portraitCache[id];
            return bi;
        }

        public BitmapImage GetChampionSplash(int championId, int skinIndex = 0)
        {
            int id = championId * 1000 + skinIndex;
            string name = null;
            if (championId != 0)
                name = _gameData.GetChampionData(championId)?.Name;
            BitmapImage bi;
            if (!splashCache.ContainsKey(id))
            {
                bi = new BitmapImage(new Uri(Path.Combine(_clientDir, string.Format(PortraitFile, name, skinIndex))));
                splashCache[id] = bi;
                bi.Freeze();
            }
            else
                bi = splashCache[id];
            return bi;
        }

        public BitmapImage GetSummonerSpellIcon(int spellId)
        {
            string filename = string.Format(SummonerSpellFile, spellId);
            if (!File.Exists(filename))
                return null;

            BitmapImage bi;
            if (!spellIconCache.ContainsKey(spellId))
            {
                bi = new BitmapImage(new Uri(Path.GetFullPath(filename)));
                spellIconCache[spellId] = bi;
                bi.Freeze();
            }
            else
                bi = spellIconCache[spellId];
            return bi;
        }

        public BitmapImage GetWardSkin(int wardId)
        {
            string filename = Path.Combine(_clientDir, string.Format(WardSkinFile, wardId));
            if (!File.Exists(filename))
                return null;

            BitmapImage bi;
            if (!wardSkinCache.ContainsKey(wardId))
            {
                bi = new BitmapImage(new Uri(Path.GetFullPath(filename)));
                wardSkinCache[wardId] = bi;
                bi.Freeze();
            }
            else
                bi = wardSkinCache[wardId];
            return bi;
        }
        
    }
}
