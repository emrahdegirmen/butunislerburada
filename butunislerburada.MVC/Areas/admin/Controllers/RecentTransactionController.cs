using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using butunislerburada.MVC.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    [LoginSetting]
    public class RecentTransactionController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public RecentTransactionController()
        {
            unitOfWork = new GenericUnitOfWork();
        }
                
        [HttpPost]
        public ActionResult Filter(DataModel model)
        {
            return RedirectToAction("List", "RecentTransaction", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "", string StatusID = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.RecentTransactionsPaged = unitOfWork.Repository<RecentTransaction>().GetList(op => op.Name.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else
            {
                model.RecentTransactionsPaged = unitOfWork.Repository<RecentTransaction>().GetList().ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}