using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
using butunislerburada.Data.Model;
using butunislerburada.MVC.Attributes;
using System;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    [LoginSetting]
    public class AdminSettingController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;
        
        public AdminSettingController()
        {
            unitOfWork = new GenericUnitOfWork();
        }
        
        [HttpPost]
        public JsonResult Delete(int Id)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            unitOfWork.Repository<Admin>().Delete(Id);
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
            return RedirectToAction("List", "Admin", new { SearchText = model.FilterText });
        }

        public ActionResult List(int? Id)
        {
            DataModel model = new DataModel();
            
            if (Id != null)
            {
                model.Admin = unitOfWork.Repository<Admin>().FirstOrDefault(x => x.ID == Id);
            }

            model.Admins = unitOfWork.Repository<Admin>().GetList(x=> x.Type == 0);

            return View(model);
        }
                
        [HttpPost]
        public JsonResult List(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            var result = 0;

            if (model.Admin.ID > 0)
            {
                var resultData = unitOfWork.Repository<Admin>().Update(model.Admin);
            }
            else
            {
                model.Admin.LastLoginDate = DateTime.Now;
                var resultData = unitOfWork.Repository<Admin>().Insert(model.Admin);
            }

            result = unitOfWork.SaveChanges();

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

    }
}