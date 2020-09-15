using System;

namespace butunislerburada.Data.Entity
{
    public class BadLinkLog : BaseEntity
    {
        public string BadLink { get; set; }
        public string Error { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
