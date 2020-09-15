using System;

namespace butunislerburada.Data.Entity
{
    public class Job : BaseEntity
    {
        public int CompanyID { get; set; }
        public int CategoryID { get; set; }
        public int CityID { get; set; }

        public string CategoryIDList { get; set; }
        public string CityIDList { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }

        public string BotPageUrl { get; set; }
        public string BotUrl { get; set; }
    }
}
