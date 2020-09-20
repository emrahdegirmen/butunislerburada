using System;

namespace butunislerburada.Data.Entity
{
    public class JobCity : BaseEntity
    {
        public int JobID { get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
    }
}
