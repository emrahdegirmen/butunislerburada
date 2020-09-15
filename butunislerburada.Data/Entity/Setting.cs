using System;

namespace butunislerburada.Data.Entity
{
    public class Setting : BaseEntity
    {
        public string MainPageTitle { get; set; }
        public string PageTitle { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string MapCode { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Mail { get; set; }

        public string Facebook{ get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Google { get; set; }
        public string Youtube { get; set; }
        public string Linkedin { get; set; }
        public string Pinterest { get; set; }

        public string MailServer { get; set; }
        public string MailAddress { get; set; }
        public string MailPassword { get; set; }
    }
}
