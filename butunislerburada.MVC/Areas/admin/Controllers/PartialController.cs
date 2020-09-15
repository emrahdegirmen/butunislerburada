using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using System.Linq;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    public class PartialController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public PartialController()
        {
            unitOfWork = new GenericUnitOfWork();
        }


        public PartialViewResult _Header()
        {
            DataModel model = new DataModel();
            
            model.Contacts = unitOfWork.Repository<Contact>().GetList(x => x.StatusID == 1);
            model.RecentTransactions = unitOfWork.Repository<RecentTransaction>().GetList().Take(10).ToList();

            return PartialView(model);
        }

        public PartialViewResult _SidebarMenu()
        {
            DataModel model = new DataModel();

            model.SingerCount = unitOfWork.Repository<Singer>().Count(x => x.StatusID == 1);
            model.LyricsCount = unitOfWork.Repository<Lyrics>().Count(x => x.StatusID == 1);
            model.ContactCount = unitOfWork.Repository<Contact>().Count(x => x.StatusID == 1);
            model.BlogCount = unitOfWork.Repository<Blog>().Count(x => x.StatusID == 1);

            return PartialView(model);
        }
    }
}