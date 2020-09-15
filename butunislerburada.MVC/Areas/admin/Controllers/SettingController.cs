using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
using butunislerburada.Data.Model;
using butunislerburada.MVC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    [LoginSetting]
    public class SettingController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public SettingController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add()
        {
            DataModel model = new DataModel();

            model.Setting = unitOfWork.Repository<Setting>().GetList().FirstOrDefault();

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Add(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            var result = 0;

            if (model.Setting.ID > 0)
            {
                unitOfWork.Repository<Setting>().Update(model.Setting);
            }
            else
            {
                unitOfWork.Repository<Setting>().Insert(model.Setting);
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