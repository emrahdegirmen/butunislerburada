using HtmlAgilityPack;
using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    public class BotSingerController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public BotSingerController()
        {
            unitOfWork = new GenericUnitOfWork();
        }
        
        string defaultUrl = "https://sarki.alternatifim.com/";

        public bool IsStop = false;

        public ActionResult GetSinger(string letter = "")
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork();

            try
            {
                if (letter != "")
                {
                    GetSingerLetter(defaultUrl + "populer-sarkicilar/" + letter);
                }
                else
                {
                    Uri uri = new Uri(defaultUrl);
                    WebClient client = new WebClient();
                    var htmlData = client.DownloadData(uri);
                    var html = Encoding.UTF8.GetString(htmlData);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    HtmlNodeCollection singerLitterList = doc.DocumentNode.SelectNodes("//ul[@class='harfListesi'][1]//li");
                    if (singerLitterList != null)
                    {
                        foreach (var item in singerLitterList)
                        {
                            string letterLink = item.SelectSingleNode(".//a").Attributes["href"].Value;
                            string letterName = item.SelectSingleNode(".//a").InnerHtml;

                            if (letterLink.Contains("/populer-sarkilar/"))
                            {
                                IsStop = true;
                            }

                            if (!IsStop)
                            {
                                GetSingerLetter(defaultUrl + letterLink);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var aa = ex.ToString();

                ErrorLog errorLog = new ErrorLog();
                errorLog.Message = ex.Message;
                errorLog.Error = ex.ToString();
                errorLog.PageName = Request.Url.AbsolutePath;
                unitOfWork.Repository<ErrorLog>().Insert(errorLog);
                unitOfWork.SaveChanges();
            }

            return View();
        }

        public int GetPageCount(HtmlAgilityPack.HtmlDocument doc)
        {
            var returnValue = 1;

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//script");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (item.InnerHtml.Contains("max_page"))
                    {
                        string[] path = item.InnerHtml.Split(',');
                        for (int i = 0; i < path.Length; i++)
                        {
                            if (path[i].Contains("max_page"))
                            {
                                if (returnValue == 1)
                                {
                                    returnValue = Convert.ToInt32(Helper.Helper.clearHtml(path[i].ToString().Replace("max_page:", "").Replace("max_page :", "")).Trim());
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        public void GetSingerLetter(string url, int page = 1)
        {
            //https://sarki.alternatifim.com/populer-sarkicilar/A
            //https://sarki.alternatifim.com/populer-sarkicilar/B

            string botPageUrl = url + "/sayfa-" + page;

            Uri uri = new Uri(botPageUrl);
            WebClient client = new WebClient();
            var htmlData = client.DownloadData(uri);
            var html = Encoding.UTF8.GetString(htmlData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var pageCount = GetPageCount(doc);


            HtmlNodeCollection htmlLyricsRemove = doc.DocumentNode.SelectNodes("//div[@class='cen']");
            if (htmlLyricsRemove != null)
            {
                foreach (var item in htmlLyricsRemove)
                {
                    item.Remove();
                }
            }


            HtmlNodeCollection singerLyricsList = doc.DocumentNode.SelectNodes("//div[@class='sarkisozu']//ul//li");
            if (singerLyricsList != null)
            {
                foreach (var item in singerLyricsList)
                {
                    string singerLink = item.SelectSingleNode(".//a").Attributes["href"].Value;
                    string singerName = item.SelectSingleNode(".//a").InnerHtml;
                    string SingerDataSize = item.SelectSingleNode(".//span").InnerHtml.Replace("(", "").Replace(")", "");

                    if (SingerDataSize != null)
                    {
                        if (!string.IsNullOrEmpty(singerName))
                        {
                            var mySingerLink = Helper.Helper.editCharacter(singerName);

                            Singer singer = new Singer();

                            var singerData = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.BotSingerLink == singerLink);
                            if (singerData == null)
                            {
                                singer.Name = singerName;
                                singer.SingerLink = mySingerLink;
                                singer.SingerDataSize = SingerDataSize;
                                singer.BotPageUrl = botPageUrl;
                                singer.BotSingerLink = singerLink;
                                singer.BotStatusID = 0;
                                unitOfWork.Repository<Singer>().Insert(singer);
                                unitOfWork.SaveChanges();
                            }
                        }
                    }
                }
            }


            if (page <= pageCount)
            {
                page = page + 1;
                GetSingerLetter(url, page);
            }
        }

        public ActionResult FixSingerLink(int pageID = 1)
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork();

            try
            {
                var fixLinkList = unitOfWork.Repository<Singer>().GetList().ToPagedList(pageID, 500).GroupBy(x => x.SingerLink).Select(x => new { SingerLink = x.Key, SingerIDCount = x.Count() });
                if (fixLinkList != null && fixLinkList.ToList().Count > 0)
                {
                    foreach (var item in fixLinkList)
                    {
                        if (item.SingerIDCount > 1)
                        {
                            var singerData = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.SingerLink == item.SingerLink);
                            if (singerData != null)
                            {
                                unitOfWork.Repository<Singer>().Delete(singerData.ID);
                                unitOfWork.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("FixSingerLink", "BotSinger", new { pageID = pageID + 1 });
                }
            }
            catch (Exception ex)
            {
                ErrorLog errorLog = new ErrorLog();
                errorLog.Message = ex.Message;
                errorLog.Error = ex.ToString();
                errorLog.PageName = Request.Url.AbsolutePath;
                unitOfWork.Repository<ErrorLog>().Insert(errorLog);
                unitOfWork.SaveChanges();
            }

            return View();
        }
    }
}