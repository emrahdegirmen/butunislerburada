using System;

namespace butunislerburada.Data.Entity
{
    public class Admin : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
        public DateTime LastLoginDate{ get; set; }
    }
}
