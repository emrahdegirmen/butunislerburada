using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace butunislerburada.MVC.Controllers
{
    public class HomeController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public HomeController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Index(int sayfa = 1)
        {

            return RedirectToAction("Index", "ElemanOnline");


            //DataModel model = new DataModel();

            //string query = "";

            //if (sayfa > 1)
            //{
            //    query = "?sy=" + sayfa;
            //}

            //Uri uri = new Uri("https://www.elemanonline.com.tr/is_ilanlari.php" + query);
            //WebClient client = new WebClient();
            //var htmlData = client.DownloadData(uri);
            //var html = Encoding.UTF8.GetString(htmlData);

            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(html);


            //var IsContinue = true;

            //HtmlNodeCollection dataList = doc.DocumentNode.SelectNodes("//h4");
            //if (dataList != null)
            //{
            //    foreach (var item in dataList)
            //    {
            //        string link = item.SelectSingleNode(".//a").Attributes["href"].Value;
            //        string title = item.SelectSingleNode(".//a").Attributes["title"].Value;

            //        IsContinue = SaveJob(link, 1, title, sayfa);
            //    }

            //    if (IsContinue)
            //    {
            //        int page = sayfa + 1;
            //        Response.Redirect("?sayfa=" + page);
            //    }
            //}

            //return View(model);
        }

        public bool SaveJob(string url, int categoryID, string jobTitle, int page)
        {
            bool returnValue = true;

            try
            {
                var data = unitOfWork.Repository<Job>().FirstOrDefault(x => x.BotLink == url);
                if (data == null)
                {
                    Uri uri = new Uri(url);
                    WebClient client = new WebClient();
                    var htmlData = client.DownloadData(uri);
                    var html = Encoding.UTF8.GetString(htmlData);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);


                    Job job = new Job();
                    job.CategoryID = categoryID;
                    job.Name = jobTitle;
                    job.BotLink = url;
                    job.BotPageLink = "https://www.elemanonline.com.tr/is_ilanlari.php?sy=" + page;

                    //string htmlCompany = doc.DocumentNode.SelectSingleNode("//h6[@class='mb-5']").InnerHtml.Trim();
                    //if (htmlCompany != null)
                    //{
                    //    job.CompanyName = Helper.Helper.clearHtml(htmlCompany);
                    //}
                    
                    string htmlDate = doc.DocumentNode.SelectSingleNode("//div[@class='pull-left hidden-xs ml-5']").InnerHtml.Trim();
                    if (htmlDate != null)
                    {
                        htmlDate = Helper.Helper.clearHtml(htmlDate);

                        if (htmlDate.Contains("Bugün"))
                        {
                            job.ReleaseDate = DateTime.Now;
                        }
                        else if (htmlDate.Contains("Dün"))
                        {
                            job.ReleaseDate = DateTime.Now.AddDays(-1);
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }


                    string htmlText = doc.DocumentNode.SelectSingleNode("//div[@id='ilan_metni']").InnerHtml.Trim();
                    if (htmlText != null)
                    {
                        job.Detail = htmlText;
                    }

                    if (returnValue)
                    {
                        var resultData = unitOfWork.Repository<Job>().Insert(job);
                        unitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return returnValue;
        }
    }
}