using PagedList;
using butunislerburada.Data.Entity;
using System.Collections.Generic;
using System.Web;

namespace butunislerburada.Data.Model
{
    public class DataModel
    {
        public Singer Singer { get; set; }
        public List<Singer> Singers { get; set; }
        public IPagedList<Singer> SingersPaged { get; set; }
        public IPagedList<Lyrics> SingersLyrics { get; set; }

        public Lyrics Lyrics { get; set; }
        public List<Lyrics> Lyricses { get; set; }
        public IPagedList<Lyrics> LyricsPaged { get; set; }


        public IPagedList<Lyrics> LastAddedLyrics { get; set; }
        public IPagedList<Lyrics> PopularLyrics { get; set; }
        public IPagedList<Lyrics> Last10Lyrics { get; set; }
        public IPagedList<Lyrics> IndexLyrics { get; set; }

        public ErrorLog ErrorLog { get; set; }
        public List<ErrorLog> ErrorLogs { get; set; }
        public IPagedList<ErrorLog> ErrorLogsPaged { get; set; }

        public BadLinkLog BadLinkLog { get; set; }
        public List<BadLinkLog> BadLinkLogs { get; set; }
        public IPagedList<BadLinkLog> BadLinkLogsPaged { get; set; }



        public IPagedList<Singer> SingersFooter { get; set; }
        public IPagedList<Lyrics> LyricsFooter { get; set; }


        public int SingerCount { get; set; }
        public int LyricsCount { get; set; }
        public int ContactCount { get; set; }
        public int BlogCount { get; set; }

        public Contact Contact { get; set; }
        public List<Contact> Contacts { get; set; }
        public IPagedList<Contact> ContactPaged { get; set; }


        public Blog Blog { get; set; }
        public List<Blog> Blogs { get; set; }
        public IPagedList<Blog> BlogPaged { get; set; }


        public RecentTransaction RecentTransaction { get; set; }
        public List<RecentTransaction> RecentTransactions { get; set; }
        public IPagedList<RecentTransaction> RecentTransactionsPaged { get; set; }

        public Setting Setting { get; set; }


        public Admin Admin { get; set; }
        public List<Admin> Admins { get; set; }



        //Ekrana hata mesajı yada mesaj yazdırmak için
        public string Message { get; set; }
        public int MessageStatusID { get; set; }


        public string PageTitle { get; set; }

        public string FilterText { get; set; }


        public string StatusID { get; set; }
        public string BotStatusID { get; set; }

        public string ReturnUrl { get; set; }
    }
}
