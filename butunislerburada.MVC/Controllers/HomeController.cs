using PagedList;
using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;

namespace butunislerburada.MVC.Controllers
{
    public class HomeController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public HomeController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Index()
        {
            DataModel model = new DataModel();
            model.SingersPaged = unitOfWork.Repository<Singer>().GetList(x => x.ImagePath != null && x.ImagePath != "").ToPagedList(1, 12);

            ViewBag.MetaDescription = "Binlerce yerli ve yabancı şarkı sözlerini sitemizden bulabilirsiniz. Şarkı sözleri burada. En güncel en trend şarkı sözlerine sitemizde bulunan arama menüsünü kullanarak ulaşabilirsiniz.";



            //List<Lyrics> IndexLyrics = new List<Lyrics>();

            //var singerList = unitOfWork.Repository<Singer>().GetList(x => x.PopularityPoint < 4 && x.BotStatusID == 1 && x.LyricsCount > 0).OrderByDescending(x => Guid.NewGuid()).ToPagedList(1, 50);
            //if (singerList != null && singerList.Count > 0)
            //{
            //    foreach (var item in singerList)
            //    {
            //        var current = item.Lyrics.OrderByDescending(x => Guid.NewGuid()).ToPagedList(1, 1);
            //        if (current != null)
            //        {
            //            IndexLyrics.Add(item.Lyrics.FirstOrDefault());
            //        }
            //    }
            //}



            List<Lyrics> IndexLyrics = new List<Lyrics>();

            var lyricsList = unitOfWork.Repository<Lyrics>().GetList().OrderByDescending(x => Guid.NewGuid()).ToPagedList(1, 50);
            if (lyricsList != null && lyricsList.Count > 0)
            {
                foreach (var item in lyricsList)
                {
                    //if (!IndexLyrics.Exists(x => x.SingerID == item.SingerID))
                    //{
                    //    IndexLyrics.Add(item);
                    //}

                    IndexLyrics.Add(item);
                }
            }
            
            if (IndexLyrics != null && IndexLyrics.Count > 0)
            {
                model.LastAddedLyrics = IndexLyrics.ToPagedList(1, 10);
                model.PopularLyrics = IndexLyrics.ToPagedList(2, 10);
                model.Last10Lyrics = IndexLyrics.ToPagedList(3, 10);
                model.IndexLyrics = IndexLyrics.ToPagedList(4, 12);
            }

            return View(model);
        }

        [Route("sanatcilar")]
        public ActionResult SingerList(int sayfa = 1)
        {
            ViewBag.Title = "Sanatçılar";
            DataModel model = new DataModel();
            model.SingersPaged = unitOfWork.Repository<Singer>().GetList(x=> x.BotStatusID == 1).ToPagedList(sayfa, 12);
            return View(model);
        }

        [Route("sarki-sozleri")]
        public ActionResult LyricsList(int sayfa = 1)
        {
            ViewBag.Title = "Şarkı Sözleri";
            DataModel model = new DataModel();            
            model.LyricsPaged = unitOfWork.Repository<Lyrics>().GetList().ToPagedList(sayfa, 12);
            return View(model);
        }

        [Route("{id}-sarki-sozleri")]
        public ActionResult LyricsFilterList(string id)
        {
            DataModel model = new DataModel();

            if (id == "populer" || id == "trend")
            {
                if (id == "populer")
                {
                    model.PageTitle = "Popüler";
                    ViewBag.Title = "Popüler Şarkı Sözleri";
                }
                else if (id == "trend")
                {
                    model.PageTitle = "Trend";
                    ViewBag.Title = "Trend Şarkı Sözleri";
                }


                List<Lyrics> IndexLyrics = new List<Lyrics>();

                var lyricsList = unitOfWork.Repository<Lyrics>().GetList().OrderByDescending(x => Guid.NewGuid()).ToPagedList(1, 500);
                if (lyricsList != null && lyricsList.Count > 0)
                {
                    foreach (var item in lyricsList)
                    {
                        if (!IndexLyrics.Exists(x => x.SingerID == item.SingerID))
                        {
                            IndexLyrics.Add(item);
                        }
                    }
                    model.Lyricses = IndexLyrics;
                }
            }
            else
            {
                model.Singer = unitOfWork.Repository<Singer>().FirstOrDefault(x => x.SingerLink == id);

                var singerLyrics = "";

                if (model.Singer.Lyrics != null)
                {
                    foreach (var item in model.Singer.Lyrics.Take(3))
                    {
                        singerLyrics += item.Name.Replace(" - "," ") + " şarkı sözü.";
                    }
                }
                
                ViewBag.Title = model.Singer.Name + " Şarkı Sözleri";
                ViewBag.MetaDescription = model.Singer.Name + " şarkı sözleri. Tüm " + model.Singer.Name + " şarkı sözleri burada. " + singerLyrics;

                var _Lyrics = unitOfWork.Repository<Lyrics>().GetList(x => x.SingerID == model.Singer.ID);
                if (_Lyrics != null && _Lyrics.Count > 0)
                {
                    model.IndexLyrics = _Lyrics.ToPagedList(1, 20);

                    if (_Lyrics.Count > 20)
                    {
                        var currentCount = (_Lyrics.Count - 20) / 3;

                        model.LastAddedLyrics = _Lyrics.ToPagedList(2, currentCount);
                        model.PopularLyrics = _Lyrics.ToPagedList(3, currentCount);
                        model.Last10Lyrics = _Lyrics.ToPagedList(4, currentCount);
                    }
                }
            }

            return View(model);
        }

        [Route("{id}-sarki-sozu")]
        public ActionResult LyricsDetail(string id)
        {
            DataModel model = new DataModel();

            var Lyrics = unitOfWork.Repository<Lyrics>().FirstOrDefault(x => x.LyricsLink == id);
            if (Lyrics != null)
            {
                model.Lyrics = Lyrics;

                ViewBag.Title = model.Lyrics.Name + " Şarkı Sözü";

                var lyricsText = "";
                if (model.Lyrics.Text != null)
                {
                    string[] text = model.Lyrics.Text.Replace("<br>", "ß").Split('ß');
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (i <= 1)
                        {
                            if (i == 1)
                            {
                                lyricsText += text[i] + "...";
                            }
                            else
                            {
                                lyricsText += text[i] + " ";
                            }
                        }
                    }
                }

                ViewBag.MetaDescription = model.Lyrics.Name.Replace(" - "," ") + " şarkı sözü. Tüm " + model.Lyrics.Singer.Name + " şarkı sözlerini sitemizden takip edebilirsiniz. " + lyricsText;



                model.SingersLyrics = unitOfWork.Repository<Lyrics>().GetList(x => x.SingerID == model.Lyrics.SingerID).ToPagedList(1, 10);

                var _Lyrics = unitOfWork.Repository<Lyrics>().GetList().OrderByDescending(x => Guid.NewGuid()).ToPagedList(1, 20);
                if (_Lyrics != null && _Lyrics.Count > 0)
                {
                    model.LastAddedLyrics = _Lyrics.ToPagedList(1, 10);
                    model.PopularLyrics = _Lyrics.ToPagedList(2, 10);
                }
            }
            else
            {
                return RedirectToAction("Page404", "Error");
            }

            return View(model);
        }
    }
}