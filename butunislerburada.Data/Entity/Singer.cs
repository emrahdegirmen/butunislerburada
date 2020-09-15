using System;
using System.Collections.Generic;

namespace butunislerburada.Data.Entity
{
    public class Singer : BaseEntity
    {
        public string Name { get; set; }
        public string SingerLink { get; set; }
        public string SingerDataSize { get; set; }
        public string BotPageUrl { get; set; }
        public string BotSingerLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastBotDate { get; set; }
        public int LyricsCount { get; set; }
        public int StatusID { get; set; }
        public int BotStatusID { get; set; }
        public string ImagePath { get; set; }

        public virtual List<Lyrics> Lyrics { get; set; }
    }
}
