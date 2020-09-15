using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    public class LoginController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public LoginController()
        {
            unitOfWork = new GenericUnitOfWork();
        }


        public ActionResult Index(string returnUrl = "")
        {
            DataModel model = new DataModel();

            var AdminList = unitOfWork.Repository<Admin>().GetList();

            if (!AdminList.Any())
            {
                var Admin1 = new Admin() { UserName = "admin", Password = "12345", Type = 0 };
                var Admin2 = new Admin() { UserName = "ed", Password = "ed.2018", Type = 1 };

                unitOfWork.Repository<Admin>().Insert(Admin1);
                unitOfWork.Repository<Admin>().Insert(Admin2);
                unitOfWork.SaveChanges();
            }

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DataModel model)
        {
            var _User = unitOfWork.Repository<Admin>().FirstOrDefault(x => x.UserName == model.Admin.UserName && x.Password == model.Admin.Password);
            if (_User != null)
            {
                HttpCookie _UserLogin = new HttpCookie("UserLogin");

                var LastLoginDate = _User.LastLoginDate;
                if (LastLoginDate.ToString() == "01.01.1900 00:00:00")
                {
                    LastLoginDate = DateTime.Now;
                }

                _UserLogin["UserID"] = _User.ID.ToString();
                _UserLogin["UserName"] = Server.UrlEncode(_User.UserName);
                _UserLogin["UserType"] = _User.Type.ToString();
                _UserLogin["LastLoginDate"] = LastLoginDate.ToString();

                int cookieDay = 1;

                if (_User.Type.ToString() == "1")
                {
                    cookieDay = 365;
                }

                _UserLogin.Expires = DateTime.Now.AddDays(cookieDay);

                Response.Cookies.Add(_UserLogin);


                _User.LastLoginDate = DateTime.Now;
                unitOfWork.Repository<Admin>().Update(_User);
                unitOfWork.SaveChanges();

                if (model.ReturnUrl != null && model.ReturnUrl.ToString() != "")
                {
                    Response.Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                Response.Write("Kullanıcı Bulunamadı");
            }

            return View();
        }
        
        public ActionResult Logout()
        {
            if (Request.Cookies["UserLogin"] != null)
            {
                HttpCookie _UserLogin = new HttpCookie("UserLogin");

                _UserLogin.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(_UserLogin);

                Response.Redirect("/admin/login?q=Exit");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}