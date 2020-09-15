using System;

namespace butunislerburada.Data.Entity
{
    public class RecentTransaction : BaseEntity
    {
        public string Name { get; set; }
        public int DataID { get; set; }
        public int DataTypeID { get; set; } // Singer-Lyrics
        public int SaveTypeID { get; set; } // Save-Update
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
