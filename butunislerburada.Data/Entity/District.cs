using System;

namespace butunislerburada.Data.Entity
{
    public class District : BaseEntity
    {
        public int CityID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }
    }
}
