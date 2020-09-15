using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
using butunislerburada.Data.Model;
using butunislerburada.MVC.Attributes;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    [LoginSetting]
    public class BlogController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public BlogController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add(int? Id)
        {
            DataModel model = new DataModel();

            if (Id != null)
            {
                model.Blog = unitOfWork.Repository<Blog>().FirstOrDefault(x => x.ID == Id);
            }

            return View(model);
        }

        [ValidateInput(false)]
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

            model.Blog.Link = Helper.Helper.editCharacter(model.Blog.Name);



            var result = 0;

            if (model.Blog.ID > 0)
            {
                if (file1 != null)
                {
                    var oldImagePath = model.Blog.ImagePath;
                    if (!string.IsNullOrEmpty(oldImagePath))
                    {
                        FileHelper.DeleteImage(this.ControllerContext.RouteData.Values["controller"].ToString(), oldImagePath);
                    }

                    model.Blog.ImagePath = FileHelper.SaveResizedImage(file1, this.ControllerContext.RouteData.Values["controller"].ToString(), model.Blog.Link, new Dictionary<int, int>() { { 800, 400 } }, new Dictionary<int, int>() { { 400, 200 } });
                }

                unitOfWork.Repository<Blog>().Update(model.Blog);
                result = unitOfWork.SaveChanges();

                Helper.Helper.saveRecentTransaction(model.Blog.Name, Convert.ToInt32(DataTypeName.Blog), model.Blog.ID, Convert.ToInt32(SaveUpdateStatus.Update));
            }
            else
            {
                if (file1 != null)
                {
                    model.Blog.ImagePath = FileHelper.SaveResizedImage(file1, this.ControllerContext.RouteData.Values["controller"].ToString(), model.Blog.Link, new Dictionary<int, int>() { { 800, 400 } }, new Dictionary<int, int>() { { 400, 200 } });
                }

                var resultData = unitOfWork.Repository<Blog>().Insert(model.Blog);
                result = unitOfWork.SaveChanges();

                Helper.Helper.saveRecentTransaction(model.Blog.Name, Convert.ToInt32(DataTypeName.Blog), resultData.ID, Convert.ToInt32(SaveUpdateStatus.Save));
            }

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

            unitOfWork.Repository<Blog>().Delete(Id);
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

            var entity = unitOfWork.Repository<Blog>().FirstOrDefault(x => x.ID == Id);
            entity.StatusID = StatusId;

            unitOfWork.Repository<Blog>().Update(entity);

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
            return RedirectToAction("List", "Blog", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "", string BotStatusID = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.BlogPaged = unitOfWork.Repository<Blog>().GetList(op => op.Name.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else
            {
                model.BlogPaged = unitOfWork.Repository<Blog>().GetList().ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}