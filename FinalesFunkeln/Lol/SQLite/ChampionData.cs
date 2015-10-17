
using SQLite;

namespace FinalesFunkeln.Lol.SQLite
{
    [Table("champions")]
    public class ChampionData
    {
        [Column("id")]
        public int Id { get; internal set; }

        [Column("name")]
        public string Name { get; internal set; }

        [Column("displayName")]
        public string DisplayName { get; internal set; }

        [Column("title")]
        public string Title { get; internal set; }

        [Column("iconPath")]
        public string IconPath { get; internal set; }

        [Column("portraitPath")]
        public string PortraitPath { get; internal set; }

        [Column("splashPath")]
        public string SplashPath { get; internal set; }

    }
}
