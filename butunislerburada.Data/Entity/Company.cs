using System;

namespace butunislerburada.Data.Entity
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }
    }
}
