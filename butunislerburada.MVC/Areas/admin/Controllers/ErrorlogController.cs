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
    public class ErrorlogController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public ErrorlogController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add(int? Id)
        {
            DataModel model = new DataModel();

            if (Id != null)
            {
                model.ErrorLog = unitOfWork.Repository<ErrorLog>().FirstOrDefault(x => x.ID == Id);
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult Add(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            if (model.ErrorLog.ID > 0)
            {
                unitOfWork.Repository<ErrorLog>().Update(model.ErrorLog);
            }
            else
            {
                unitOfWork.Repository<ErrorLog>().Insert(model.ErrorLog);
            }

            var result = unitOfWork.SaveChanges();

            if (result > 0)
            {
                JsonResult.IsSuccess = 1;
                JsonResult.Message = "Kayıt Başarıyla Kaydedildi";
            }
            else
            {
                JsonResult.IsSuccess = 0;
                JsonResult.Message = "Kayıt Esnasında Sorun Oluştu.";
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Delete(int Id)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            unitOfWork.Repository<ErrorLog>().Delete(Id);
            var result = unitOfWork.SaveChanges();

            if (result > 0)
            {
                JsonResult.IsSuccess = 1;
                JsonResult.Message = "Kayıt Başarıyla Silindi";
            }
            else
            {
                JsonResult.IsSuccess = 0;
                JsonResult.Message = "Silme Esnasında Sorun Oluştu.";
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
                
        [HttpPost]
        public ActionResult Filter(DataModel model)
        {
            return RedirectToAction("List", "ErrorLog", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "", string StatusID = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.ErrorLogsPaged = unitOfWork.Repository<ErrorLog>().GetList(op => op.Message.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else
            {
                model.ErrorLogsPaged = unitOfWork.Repository<ErrorLog>().GetList().ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}