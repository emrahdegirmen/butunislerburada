using System;

namespace butunislerburada.Data.Entity
{
    public class ErrorLog : BaseEntity
    {
        public string Message { get; set; }
        public string Error { get; set; } 
        public string PageName { get; set; } 
        public string LastUrl { get; set; }
        public int LastPageID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
