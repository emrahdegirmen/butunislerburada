using System;

namespace butunislerburada.Data.Entity
{
    public class Blog : BaseEntity
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
