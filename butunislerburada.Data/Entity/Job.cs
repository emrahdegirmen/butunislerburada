using System;

namespace butunislerburada.Data.Entity
{
    public class Job : BaseEntity
    {
        public int CompanyID { get; set; }
        public int CategoryID { get; set; }
        
        public string Name { get; set; }
        public string Detail { get; set; }

        public int CityID { get; set; }
        public int DistrictID { get; set; }

        public int WorkingWayID { get; set; }
        public int GenderID { get; set; }
        public int CountOfPersons { get; set; }
        public int Age1 { get; set; }
        public int Age2 { get; set; }

        public string Link { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }

        public string BotPageLink { get; set; }
        public string BotLink { get; set; }
    }
}
