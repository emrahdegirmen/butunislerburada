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
    public class ContactController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public ContactController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add(int? Id)
        {
            DataModel model = new DataModel();

            if (Id != null)
            {
                model.Contact = unitOfWork.Repository<Contact>().FirstOrDefault(x => x.ID == Id);
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult Add(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            if (model.Contact.ID > 0)
            {
                unitOfWork.Repository<Contact>().Update(model.Contact);
            }
            else
            {
                unitOfWork.Repository<Contact>().Insert(model.Contact);
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

            unitOfWork.Repository<Contact>().Delete(Id);
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
        public JsonResult ChangeStatus(int Id, int StatusId)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            var entity = unitOfWork.Repository<Contact>().FirstOrDefault(x => x.ID == Id);
            entity.StatusID = StatusId;

            unitOfWork.Repository<Contact>().Update(entity);

            var result = unitOfWork.SaveChanges();

            if (result > 0)
            {
                JsonResult.IsSuccess = 1;
                JsonResult.Message = "Durum Başarıyla Değiştirildi";
            }
            else
            {
                JsonResult.IsSuccess = 0;
                JsonResult.Message = "Durum Değiştirme Esnasında Sorun Oluştu.";
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Filter(DataModel model)
        {
            return RedirectToAction("List", "Contact", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "", string StatusID = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.ContactPaged = unitOfWork.Repository<Contact>().GetList(op => op.NameSurname.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else if (StatusID != "")
            {
                model.StatusID = StatusID;
                model.ContactPaged = unitOfWork.Repository<Contact>().GetList(op => op.StatusID.ToString() == StatusID).ToPagedList(sayfa, 50);
            }
            else
            {
                model.ContactPaged = unitOfWork.Repository<Contact>().GetList(op => op.StatusID == 1).ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}