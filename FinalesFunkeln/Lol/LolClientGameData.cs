using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.Lol.SQLite;
using SQLite;

namespace FinalesFunkeln.Lol
{
    public class LolClientGameData
    {
        readonly SQLiteConnection _db;

        readonly Dictionary<int, ChampionData> _idToChampion = new Dictionary<int, ChampionData>();
        readonly Dictionary<string, ChampionData> _internalNameToChampion = new Dictionary<string, ChampionData>();
        internal LolClientGameData(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
            
        }

        public ChampionData GetChampionData(int id)
        {
            if (_idToChampion.ContainsKey(id))
                return _idToChampion[id];
            var ch = _db.Query<ChampionData>("SELECT * FROM champions WHERE id = ? LIMIT 1", id);
            if (ch.Count > 0)
            {
                _internalNameToChampion[ch[0].Name] = _idToChampion[ch[0].Id] = ch[0];
                return ch[0];
            }
            return null;
        }
        public ChampionData GetChampionData(string internalName)
        {
            if (_internalNameToChampion.ContainsKey(internalName))
                return _internalNameToChampion[internalName];
            var ch = _db.Query<ChampionData>("SELECT * FROM champions WHERE name = ? LIMIT 1", internalName);
            if (ch.Count > 0)
            {
                _internalNameToChampion[ch[0].Name] = _idToChampion[ch[0].Id] = ch[0];
                return ch[0];
            }
            return null;
        }

        ~LolClientGameData()
        {
            _db.Close();
            _db.Dispose();
        }
        //*/
    }
}
