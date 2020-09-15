using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using butunislerburada.MVC.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    [LoginSetting]
    public class HomeController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public HomeController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Index()
        {
            DataModel model = new DataModel();

            model.SingerCount = unitOfWork.Repository<Singer>().Count(x => x.StatusID == 1);
            model.LyricsCount = unitOfWork.Repository<Lyrics>().Count(x => x.StatusID == 1);
            model.ContactCount = unitOfWork.Repository<Contact>().Count(x => x.StatusID == 1);
            model.BlogCount = unitOfWork.Repository<Blog>().Count(x => x.StatusID == 1);

            model.RecentTransactions = unitOfWork.Repository<RecentTransaction>().GetList().Take(20).ToList();

            return PartialView(model);
        }
    }
}