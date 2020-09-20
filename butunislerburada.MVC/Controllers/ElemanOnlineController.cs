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
    public class ElemanOnlineController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;

        public ElemanOnlineController()
        {
            unitOfWork = new GenericUnitOfWork();
        }

        public ActionResult Index(int sayfa = 1)
        {
            DataModel model = new DataModel();

            string query = "";

            if (sayfa > 1)
            {
                query = "?sy=" + sayfa;
            }

            Uri uri = new Uri("https://www.elemanonline.com.tr/is_ilanlari.php" + query);
            WebClient client = new WebClient();
            var htmlData = client.DownloadData(uri);
            var html = Encoding.UTF8.GetString(htmlData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);


            var IsContinue = true;

            HtmlNodeCollection dataList = doc.DocumentNode.SelectNodes("//h4");
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string link = item.SelectSingleNode(".//a").Attributes["href"].Value;
                    string title = item.SelectSingleNode(".//a").Attributes["title"].Value;

                    IsContinue = SaveJob(link, title, sayfa);
                }

                if (IsContinue)
                {
                    int page = sayfa + 1;
                    return RedirectToAction("Index", "ElemanOnline", new { sayfa = page });
                }
            }

            return View(model);
        }

        public bool SaveJob(string url, string jobTitle, int page)
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

                    string htmlDate = doc.DocumentNode.SelectSingleNode("//div[@class='pull-left hidden-xs ml-5']").InnerHtml.Trim();
                    if (htmlDate != null)
                    {
                        htmlDate = Helper.Helper.clearHtml(htmlDate);

                        if (htmlDate.Contains("Bugün"))
                            job.ReleaseDate = DateTime.Now;
                        else if (htmlDate.Contains("Dün"))
                            job.ReleaseDate = DateTime.Now.AddDays(-1);
                        else
                            returnValue = false;
                    }

                    if (returnValue)
                    {
                        List<JobCity> jobCityList = new List<JobCity>();

                        HtmlNodeCollection htmlCompanyLink = doc.DocumentNode.SelectNodes("//div[@class='col-lg-10 col-xs-12']");
                        if (htmlCompanyLink != null)
                        {
                            var botLink = htmlCompanyLink.FirstOrDefault().SelectSingleNode(".//a").Attributes["href"].Value;

                            var checkData = unitOfWork.Repository<Company>().FirstOrDefault(x => x.BotLink == botLink);
                            if (checkData != null)
                                job.CompanyID = checkData.ID;
                            else
                            {
                                Company company = new Company();
                                company.BotLink = botLink;

                                string htmlCompany = doc.DocumentNode.SelectSingleNode("//h6[@class='mb-5']").InnerHtml.Trim();
                                if (htmlCompany != null)
                                {
                                    company.Name = Helper.Helper.clearHtml(htmlCompany);
                                    company.Link = Helper.Helper.editCharacter(company.Name);
                                }

                                var resultData = unitOfWork.Repository<Company>().Insert(company);
                                unitOfWork.SaveChanges();
                                job.CompanyID = resultData.ID;
                            }
                        }

                        HtmlNodeCollection htmlDetailData = doc.DocumentNode.SelectNodes("//ul[@class='ilan_bilgi_ozet']//li");
                        if (htmlDetailData != null)
                        {
                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Kategori")))
                            {
                                Category category = new Category();
                                category.Name = Helper.Helper.clearHtml(item.SelectSingleNode(".//dt").InnerHtml);
                                category.Link = Helper.Helper.editCharacter(category.Name);

                                category.SimilarName = category.Name;
                                category.SimilarNameLink = category.Link;

                                var checkData = unitOfWork.Repository<Category>().FirstOrDefault(x => x.Link == category.Link);
                                if (checkData != null)
                                    job.CategoryID = checkData.ID;
                                else
                                {
                                    var resultData = unitOfWork.Repository<Category>().Insert(category);
                                    unitOfWork.SaveChanges();
                                    job.CategoryID = resultData.ID;
                                }
                            }

                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Şehir / İlçe")))
                            {
                                HtmlAgilityPack.HtmlDocument docCityData = new HtmlAgilityPack.HtmlDocument();
                                docCityData.LoadHtml(item.InnerHtml);

                                HtmlNodeCollection htmlCityData = docCityData.DocumentNode.SelectNodes(".//dt//li");
                                if (htmlCityData != null)
                                {
                                    int Counter = 0;

                                    foreach (var itemCity in htmlCityData)
                                    {
                                        JobCity jobCity = new JobCity();

                                        var cityFullName = Helper.Helper.clearHtml(itemCity.InnerHtml);
                                        var cityName = cityFullName;
                                        var districtName = "";

                                        if (cityFullName.Contains("-"))
                                        {
                                            string[] path = cityFullName.Split('-');
                                            if (path != null)
                                            {
                                                cityName = path[0].ToString().Trim();
                                                districtName = path[1].ToString().Trim();
                                            }
                                        }

                                        if (cityName.Contains("İst."))
                                            cityName = cityName.Replace("İst.", "İstanbul");

                                        City city = new City();
                                        city.Name = Helper.Helper.clearHtml(cityName);
                                        city.Link = Helper.Helper.editCharacter(city.Name);


                                        var CityID = 0;
                                        var DistrictID = 0;

                                        var checkData = unitOfWork.Repository<City>().FirstOrDefault(x => x.Link == city.Link);
                                        if (checkData != null)
                                        {
                                            CityID = checkData.ID;
                                        }
                                        else
                                        {
                                            var resultData = unitOfWork.Repository<City>().Insert(city);
                                            unitOfWork.SaveChanges();
                                            CityID = resultData.ID;
                                        }

                                        District district = new District();
                                        district.CityID = job.CityID;
                                        district.Name = Helper.Helper.clearHtml(districtName);
                                        district.Link = Helper.Helper.editCharacter(district.Name);

                                        var checkDataDistrict = unitOfWork.Repository<District>().FirstOrDefault(x => x.Link == district.Link);
                                        if (checkDataDistrict != null)
                                        {
                                            DistrictID = checkDataDistrict.ID;
                                        }
                                        else
                                        {
                                            var resultData = unitOfWork.Repository<District>().Insert(district);
                                            unitOfWork.SaveChanges();
                                            DistrictID = resultData.ID;
                                        }

                                        if (Counter > 0)
                                        {
                                            jobCityList.Add(jobCity);
                                        }
                                        else
                                        {
                                            job.CityID = CityID;
                                            job.DistrictID = DistrictID;
                                        }

                                        Counter++;
                                    }
                                }                                
                            }

                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Çalışma Şekli")))
                            {
                                WorkingWay workingWay = new WorkingWay();
                                workingWay.Name = Helper.Helper.clearHtml(item.SelectSingleNode(".//dt").InnerHtml);
                                workingWay.Link = Helper.Helper.editCharacter(workingWay.Name);

                                var checkData = unitOfWork.Repository<WorkingWay>().FirstOrDefault(x => x.Link == workingWay.Link);
                                if (checkData != null)
                                    job.WorkingWayID = checkData.ID;
                                else
                                {
                                    var resultData = unitOfWork.Repository<WorkingWay>().Insert(workingWay);
                                    unitOfWork.SaveChanges();
                                    job.WorkingWayID = resultData.ID;
                                }
                            }

                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Cinsiyet")))
                            {
                                Gender gender = new Gender();
                                gender.Name = Helper.Helper.clearHtml(item.SelectSingleNode(".//dt").InnerHtml);
                                gender.Link = Helper.Helper.editCharacter(gender.Name);

                                var checkData = unitOfWork.Repository<Gender>().FirstOrDefault(x => x.Link == gender.Link);
                                if (checkData != null)
                                    job.GenderID = checkData.ID;
                                else
                                {
                                    var resultData = unitOfWork.Repository<Gender>().Insert(gender);
                                    unitOfWork.SaveChanges();
                                    job.GenderID = resultData.ID;
                                }
                            }

                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Yaş")))
                            {
                                var ageFullName = item.SelectSingleNode(".//dt").InnerHtml;

                                if (ageFullName.Contains("-"))
                                {
                                    string[] path = ageFullName.Split('-');
                                    if (path != null)
                                    {
                                        job.Age1 = Convert.ToInt32(path[0].ToString().Trim());
                                        job.Age2 = Convert.ToInt32(path[1].ToString().Trim());
                                    }
                                }
                                else
                                {
                                    job.Age1 = Convert.ToInt32(ageFullName.ToString().Trim());
                                }
                            }

                            foreach (var item in htmlDetailData.Where(x => x.InnerHtml.Contains("Alınacak Kişi Sayısı")))
                                job.CountOfPersons = Convert.ToInt32(item.SelectSingleNode(".//dt").InnerHtml);
                        }

                        job.Name = jobTitle;
                        job.Link = Helper.Helper.editCharacter(job.Name) + "-is-ilani";
                        job.BotLink = url;
                        job.BotPageLink = "https://www.elemanonline.com.tr/is_ilanlari.php?sy=" + page;

                        string htmlDetail = doc.DocumentNode.SelectSingleNode("//div[@id='ilan_metni']").InnerHtml.Trim();
                        if (htmlDetail != null)
                            job.Detail = htmlDetail;

                        var resultDataJob = unitOfWork.Repository<Job>().Insert(job);
                        unitOfWork.SaveChanges();

                        job.Link = Helper.Helper.editCharacter(job.Name) + "-is-ilani-" + resultDataJob.ID;

                        unitOfWork.Repository<Job>().Update(job);
                        unitOfWork.SaveChanges();


                        if (jobCityList.Any())
                        {
                            foreach (var item in jobCityList)
                            {
                                item.JobID = resultDataJob.ID;
                                unitOfWork.Repository<JobCity>().Insert(item);
                            }
                        }
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