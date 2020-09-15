using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
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
    public class SingerController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public SingerController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add(int? Id)
        {
            DataModel model = new DataModel();

            if (Id != null)
            {
                model.Singer = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.ID == Id);
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult Add(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            HttpPostedFileBase file1 = null;

            foreach (string inputTagName in System.Web.HttpContext.Current.Request.Files)
            {
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(System.Web.HttpContext.Current.Request.Files[inputTagName]);

                if (inputTagName == "ImgFilePath")
                {
                    file1 = filebase;
                }
            }

            model.Singer.SingerLink = Helper.Helper.editCharacter(model.Singer.Name);
            
            if (model.Singer.ID > 0)
            {
                if (file1 != null)
                {
                    var oldImagePath = model.Singer.ImagePath;
                    if (!string.IsNullOrEmpty(oldImagePath))
                    {
                        FileHelper.DeleteImage(this.ControllerContext.RouteData.Values["controller"].ToString(), oldImagePath);
                    }

                    model.Singer.ImagePath = FileHelper.SaveResizedImage(file1, this.ControllerContext.RouteData.Values["controller"].ToString(), model.Singer.SingerLink, new Dictionary<int, int>() { { 800, 800 } }, new Dictionary<int, int>() { { 400, 400 } });
                }

                unitOfWork.Repository<Singer>().Update(model.Singer);

                Helper.Helper.saveRecentTransaction(model.Singer.Name, Convert.ToInt32(DataTypeName.Singer), model.Singer.ID, Convert.ToInt32(SaveUpdateStatus.Update));
            }
            else
            {
                if (file1 != null)
                {
                    model.Singer.ImagePath = FileHelper.SaveResizedImage(file1, this.ControllerContext.RouteData.Values["controller"].ToString(), model.Singer.SingerLink, new Dictionary<int, int>() { { 800, 800 } }, new Dictionary<int, int>() { { 400, 400 } });
                }

                unitOfWork.Repository<Singer>().Insert(model.Singer);


                Helper.Helper.saveRecentTransaction(model.Singer.Name, Convert.ToInt32(DataTypeName.Singer), model.Singer.ID, Convert.ToInt32(SaveUpdateStatus.Save));
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

            unitOfWork.Repository<Singer>().Delete(Id);
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

            var entity = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.ID == Id);
            entity.StatusID = StatusId;
            unitOfWork.Repository<Singer>().Update(entity);
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
        public JsonResult ChangeBotStatus(int Id, int StatusId)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            var entity = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.ID == Id);
            entity.BotStatusID = StatusId;
            unitOfWork.Repository<Singer>().Update(entity);
            var result = unitOfWork.SaveChanges();

            if (result > 0)
            {
                if (StatusId == 1)
                {
                    Helper.Helper.saveRecentTransaction(entity.Name, Convert.ToInt32(DataTypeName.Singer), Id, Convert.ToInt32(SaveUpdateStatus.Update));
                }                

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
            return RedirectToAction("List", "Singer", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "", string BotStatusID = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.SingersPaged = unitOfWork.Repository<Singer>().GetList(op => op.Name.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else if (BotStatusID != "")
            {
                model.BotStatusID = BotStatusID;
                model.SingersPaged = unitOfWork.Repository<Singer>().GetList(op => op.BotStatusID.ToString() == BotStatusID).ToPagedList(sayfa, 50);
            }
            else
            {
                model.SingersPaged = unitOfWork.Repository<Singer>().GetList(op => op.BotStatusID == 1).ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}