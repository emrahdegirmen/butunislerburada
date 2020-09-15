using HtmlAgilityPack;
using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace butunislerburada.MVC.Areas.admin.Controllers
{
    public class BotController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public BotController()
        {
            unitOfWork = new GenericUnitOfWork();
        }
        
        string defaultUrl = "https://sarki.alternatifim.com/";
        string lastUrl = "";

        public ActionResult GetLyrics()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork();

            try
            {
                var getSingerList = unitOfWork.Repository<Singer>().GetList(x => x.LastBotDate.ToString() != "1900-01-01 00:00:00.000" && x.BotStatusID == 1).OrderBy(x=> x.CreatedDate).ToPagedList(1, 100);
                if (getSingerList != null)
                {
                    foreach (var item in getSingerList)
                    {
                        lastUrl = "https://sarki.alternatifim.com/" + item.BotSingerLink;
                        GetSingerLyrics("https://sarki.alternatifim.com/" + item.BotSingerLink, item.ID, item.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog errorLog = new ErrorLog();
                errorLog.Message = ex.Message;
                errorLog.Error = ex.ToString();
                errorLog.PageName = Request.Url.AbsolutePath;
                errorLog.LastUrl = lastUrl;
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


        public int LyricsCount = 0;

        public void GetSingerLyrics(string url, int singerID, string singerName, int page = 1)
        {
            //https://sarki.alternatifim.com/sarkici/abluka-alarm
            //https://sarki.alternatifim.com/sarkici/sagopa-kajmer

            if (page == 1)
            {
                LyricsCount = 0;
            }

            Uri uri = new Uri(url + "/sayfa-" + page);
            WebClient client = new WebClient();
            var htmlData = client.DownloadData(uri);
            var html = Encoding.UTF8.GetString(htmlData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var pageCount = GetPageCount(doc);

            HtmlNodeCollection singerLyricsList = doc.DocumentNode.SelectNodes("//div[@class='sarkisozu']//ul//li");
            if (singerLyricsList != null)
            {
                foreach (var item in singerLyricsList)
                {
                    string lyricsLink = item.SelectSingleNode(".//a").Attributes["href"].Value;
                    string lyricsName = item.SelectSingleNode(".//span").InnerHtml.Replace("(", "").Replace(")", "");

                    if (!string.IsNullOrEmpty(lyricsName))
                    {
                        SaveLyrics(defaultUrl + lyricsLink, singerID, singerName);
                    }
                }
            }

            LyricsCount += singerLyricsList.Count();

            if (pageCount > 1 && page < pageCount)
            {
                page = page + 1;
                GetSingerLyrics(url, singerID, singerName, page);
            }


            if (page == pageCount)
            {
                var singerData = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.ID == singerID);
                if (singerData != null)
                {
                    singerData.LastBotDate = DateTime.Now;
                    singerData.LyricsCount = LyricsCount;
                    unitOfWork.Repository<Singer>().Update(singerData);
                    unitOfWork.SaveChanges();
                }
            }
        }

        public void SaveLyrics(string url, int SingerID, string singerName)
        {
            try
            {
                //https://sarki.alternatifim.com/sarkici/sagopa-kajmer/366-gun

                var badLinkCheck = unitOfWork.Repository<BadLinkLog>().FirstOrDefault(x => x.BadLink == url);
                if (badLinkCheck == null)
                {
                    var lyricsData = unitOfWork.Repository<Lyrics>().FirstOrDefault(x => x.BotUrl == url);
                    if (lyricsData == null)
                    {
                        Uri uri = new Uri(url);
                        WebClient client = new WebClient();
                        var htmlData = client.DownloadData(uri);
                        var html = Encoding.UTF8.GetString(htmlData);

                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);



                        Lyrics lyrics = new Lyrics();
                        lyrics.SingerID = SingerID;
                        lyrics.BotUrl = url;

                        string htmlLyricsName = doc.DocumentNode.SelectSingleNode("//h1[@class='baslik']").InnerHtml;
                        if (htmlLyricsName != null)
                        {
                            lyrics.Name = htmlLyricsName;
                            lyrics.LyricsLink = Helper.Helper.editCharacter(htmlLyricsName);
                        }


                        HtmlNodeCollection htmlLyricsRemove = doc.DocumentNode.SelectNodes("//div[@class='adf300']");
                        if (htmlLyricsRemove != null)
                        {
                            foreach (var item in htmlLyricsRemove)
                            {
                                item.Remove();
                            }
                        }


                        string htmlLyricsText = doc.DocumentNode.SelectSingleNode("//div[@class='sarkisozu']").InnerHtml.Trim();
                        if (htmlLyricsText != null)
                        {
                            htmlLyricsText = htmlLyricsText.Replace("ß", "&").Replace("<script>", "ß");
                            string[] text = htmlLyricsText.Split('ß');
                            htmlLyricsText = text[0].ToString().Trim();

                            lyrics.Text = htmlLyricsText;
                        }

                        var resultData = unitOfWork.Repository<Lyrics>().Insert(lyrics);
                        unitOfWork.SaveChanges();



                        Helper.Helper.saveRecentTransaction(lyrics.Name, Convert.ToInt32(DataTypeName.Lyrics), resultData.ID, Convert.ToInt32(SaveUpdateStatus.Save));
                    }
                }
            }
            catch (Exception ex)
            {
                BadLinkLog badLinkLog = new BadLinkLog();
                badLinkLog.BadLink = url;
                badLinkLog.Error = ex.Message;
                unitOfWork.Repository<BadLinkLog>().Insert(badLinkLog);
                unitOfWork.SaveChanges();
            }
        }


        public ActionResult FixLyricsLink(int pageID = 1)
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork();

            try
            {
                var fixLinkList = unitOfWork.Repository<Lyrics>().GetList().ToPagedList(pageID, 500).GroupBy(x => x.LyricsLink).Select(x => new { LyricsLink = x.Key, LyricsIDCount = x.Count() });
                if (fixLinkList != null && fixLinkList.ToList().Count > 0)
                {
                    foreach (var item in fixLinkList)
                    {
                        if (item.LyricsIDCount > 1)
                        {
                            var lyricsData = unitOfWork.Repository<Lyrics>().FirstOrDefault(x => x.LyricsLink == item.LyricsLink);
                            if (lyricsData != null)
                            {
                                unitOfWork.Repository<Lyrics>().Delete(lyricsData.ID);
                                unitOfWork.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("FixLyricsLink", "Bot", new { pageID = pageID + 1 });
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