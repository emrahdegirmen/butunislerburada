using System;

namespace butunislerburada.Data.Entity
{
    public class Lyrics : BaseEntity
    {
        public int SingerID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string LyricsLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }
        public string BotUrl { get; set; }

        public virtual Singer Singer { get; set; }
    }
}
