using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            return new BitmapImage(new Uri(Path.Combine(_clientDir, name == null ? RandomIconFile : string.Format(IconFile, name))));
        }

        public BitmapImage GetChampionPortrait(int championId, int skinIndex = 0)
        {
            return new BitmapImage(new Uri(Path.Combine(_clientDir, string.Format(PortraitFile, _gameData.GetChampionData(championId)?.Name, skinIndex))));
        }

        public BitmapImage GetChampionSplash(int championId, int skinIndex = 0)
        {
            return new BitmapImage(new Uri(Path.Combine(_clientDir, string.Format(SplashFile, _gameData.GetChampionData(championId)?.Name, skinIndex))));
        }

        public BitmapImage GetSummonerSpellIcon(int spellId)
        {
            string filename = string.Format(SummonerSpellFile, spellId);
            if (!File.Exists(filename))
                return null;
            else
                return new BitmapImage(new Uri(Path.GetFullPath(filename)));
        }

        public BitmapImage GetWardSkin(int wardId)
        {
            string filename = Path.Combine(_clientDir, string.Format(WardSkinFile, wardId));
            return !File.Exists(filename) ? null : new BitmapImage(new Uri(Path.GetFullPath(filename)));
        }
    }
}
