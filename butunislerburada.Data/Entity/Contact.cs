using System;

namespace butunislerburada.Data.Entity
{
    public class Contact : BaseEntity
    {
        public string NameSurname { get; set; }
        public string Mail { get; set; }
        public string Subject { get; set; }
        public string Messages { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }
    }
}
