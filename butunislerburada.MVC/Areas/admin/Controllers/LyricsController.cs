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
    public class LyricsController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;
        
        public LyricsController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Add(int? Id)
        {
            DataModel model = new DataModel();

            model.Singers = unitOfWork.Repository<Singer>().GetList();

            if (Id != null)
            {
                model.Lyrics = unitOfWork.Repository<Lyrics>().FirstOrDefault(x => x.ID == Id);
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult Add(DataModel model)
        {
            JsonResultModel JsonResult = new JsonResultModel();

            model.Lyrics.LyricsLink = Helper.Helper.editCharacter(model.Lyrics.Name);

            var result = 0;

            if (model.Lyrics.ID > 0)
            {
                var resultData = unitOfWork.Repository<Lyrics>().Update(model.Lyrics);
                result = unitOfWork.SaveChanges();
                Helper.Helper.saveRecentTransaction(model.Lyrics.Name, Convert.ToInt32(DataTypeName.Lyrics), model.Lyrics.ID, Convert.ToInt32(SaveUpdateStatus.Update));
            }
            else
            {
                var resultData = unitOfWork.Repository<Lyrics>().Insert(model.Lyrics);
                result = unitOfWork.SaveChanges();
                Helper.Helper.saveRecentTransaction(model.Lyrics.Name, Convert.ToInt32(DataTypeName.Lyrics), resultData.ID, Convert.ToInt32(SaveUpdateStatus.Save));
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

            unitOfWork.Repository<Lyrics>().Delete(Id);
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

            var entity = unitOfWork.Repository<Lyrics>().FirstOrDefault(x => x.ID == Id);
            entity.StatusID = StatusId;

            unitOfWork.Repository<Lyrics>().Update(entity);

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
            return RedirectToAction("List", "Lyrics", new { SearchText = model.FilterText });
        }

        public ActionResult List(int sayfa = 1, string SearchText = "")
        {
            DataModel model = new DataModel();

            if (SearchText != "")
            {
                model.FilterText = SearchText;
                model.LyricsPaged = unitOfWork.Repository<Lyrics>().GetList(op => op.Name.Contains(SearchText)).ToPagedList(sayfa, 50);
            }
            else
            {
                model.LyricsPaged = unitOfWork.Repository<Lyrics>().GetList().ToPagedList(sayfa, 50);
            }

            return View(model);
        }
    }
}