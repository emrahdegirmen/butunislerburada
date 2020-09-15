using butunislerburada.Business.UnitOfWork;
using butunislerburada.Data.Entity;
using butunislerburada.Data.Enum;
using butunislerburada.Data.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Mail;
using System.Web;

namespace butunislerburada.MVC.Helper
{
    public class Helper
    {

        public static void saveRecentTransaction(string Name, int DataTypeID, int DataID, int SaveTypeID)
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork();

            RecentTransaction recentTransaction = new RecentTransaction();

            recentTransaction.Name = Name;
            recentTransaction.DataID = DataID;
            recentTransaction.DataTypeID = DataTypeID;
            recentTransaction.SaveTypeID = SaveTypeID;
            recentTransaction.CreatedDate = DateTime.Now;
            recentTransaction.UserID = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserLogin"]["UserID"]);

            unitOfWork.Repository<RecentTransaction>().Insert(recentTransaction);
            unitOfWork.SaveChanges();

            var List = unitOfWork.Repository<RecentTransaction>().GetList(x => x.DataTypeID == DataTypeID);

            var recentTransactionList = List.ToPagedList(1, 20);
            var recentTransactionListDelete = List.ToPagedList(2, 20);
            if (recentTransactionListDelete != null && recentTransactionListDelete.Count > 0)
            {
                foreach (var item in recentTransactionListDelete)
                {
                    unitOfWork.Repository<RecentTransaction>().Delete(item.ID);
                    unitOfWork.SaveChanges();
                }
            }
        }

        public static string editCharacter(string source)
        {
            string returnValue = clearHtml(source);

            returnValue = returnValue.ToLower();
            returnValue = returnValue.Replace(" ", "-");
            returnValue = returnValue.Replace("ç", "c");
            returnValue = returnValue.Replace("ğ", "g");
            returnValue = returnValue.Replace("ı", "i");
            returnValue = returnValue.Replace("ö", "o");
            returnValue = returnValue.Replace("ş", "s");
            returnValue = returnValue.Replace("ü", "u");
            returnValue = returnValue.Replace("\"", "");
            returnValue = returnValue.Replace("|", "");
            returnValue = returnValue.Replace("/", "");
            returnValue = returnValue.Replace("(", "");
            returnValue = returnValue.Replace(")", "");
            returnValue = returnValue.Replace("{", "");
            returnValue = returnValue.Replace("}", "");
            returnValue = returnValue.Replace("%", "");
            returnValue = returnValue.Replace("&", "");
            returnValue = returnValue.Replace("+", "");
            returnValue = returnValue.Replace(".", "-");
            returnValue = returnValue.Replace("?", "");
            returnValue = returnValue.Replace(",", "");
            returnValue = returnValue.Replace(":", "-");
            returnValue = returnValue.Replace("’", "-");
            returnValue = returnValue.Replace("'", "-");
            returnValue = returnValue.Replace("‘", "-");

            returnValue = returnValue.Replace("----", "-");
            returnValue = returnValue.Replace("---", "-");
            returnValue = returnValue.Replace("--", "-");

            if (returnValue != "")
            {
                if (returnValue.Substring(0, 1) == "-")
                {
                    returnValue = returnValue.Remove(0, 1);
                }
            }

            return returnValue;
        }

        public static string clearHtml(string source)
        {
            string result = "";

            result = source.Replace("\r", " ");
            result = result.Replace("\n", " ");
            result = result.Replace("\t", string.Empty);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"( )+", " ");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>).*(</script>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*td([^>])*>", "\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*br( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*li( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*div([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*tr([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*p([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @" ", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&lt;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&gt;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&(.{2,6});", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = result.Replace("\n", "\r");
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\t)", "\t\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\r)", "\t\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\t)", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string breaks = "\r\r\r";
            string tabs = "\t\t\t\t\t";
            for (int index = 0; index < result.Length; index++)
            {
                result = result.Replace(breaks, "\r\r");
                result = result.Replace(tabs, "\t\t\t\t");
                breaks = breaks + "\r";
                tabs = tabs + "\t";
            }

            return result;
        }

        public static bool TestEmailRegex(string emailAddress)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
            + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
            + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
            + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
            + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);
            bool isStrictMatch = reStrict.IsMatch(emailAddress);
            return isStrictMatch;
        }


        public static string restrictWord(string word, int count)
        {
            string returnValue = "";
            if (!string.IsNullOrEmpty(word))
            {
                returnValue = clearHtml(word);

                if (returnValue.Length > count)
                {
                    returnValue = returnValue.Substring(0, count) + "...";
                }
            }

            return returnValue;
        }

        //public static void MailSender(string body)
        //{
        //    var fromAddress = new MailAddress("mailadresiniz@gmail.com");
        //    var toAddress = new MailAddress("mailadresiniz@gmail.com");
        //    const string subject = "Site Adı | Sitenizden Gelen İletişim Formu";
        //    using (var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, "Mail_Sifresi")
        //    })
        //    {
        //        using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
        //        {
        //            smtp.Send(message);
        //        }
        //    }
        //}


        public static string mailSender(string mailTitle, string mailBody)
        {
            string returnValue = "";

            try
            {
                string mailServer = "mail.sarkisozleriburada.com";
                string mailAddress = "info@sarkisozleriburada.com";
                string mailPassword = "ed.2019!!";

                string siteName = "sarkisozleriburada.com";
                string siteLink = "https://www.sarkisozleriburada.com";

                MailAddress gonderen = new MailAddress(mailAddress, siteLink);
                MailAddress alan = new MailAddress(mailAddress, siteLink);
                MailMessage eposta = new MailMessage(gonderen, alan);

                AlternateView htmlView;

                htmlView = AlternateView.CreateAlternateViewFromString("<html> <head> </head> <body style='font-family: Tahoma, Arial; font-size: 11px;color:black'>  " +
                 " <div align='center'> " +
                 " <table border='0' width='600' cellspacing='0' cellpadding='0'> " +
                     " <tbody> " +

                       " <tr> " +
                            " <td width='600' bgcolor='#2577c2' align='center' colspan='3' height='50'> <b style='font-size:16px;padding:20px 0;display:block;color:#fff;'> " + siteName + " " + mailTitle + "</b> </td> " +
                       " </tr> " +

                        " <tr> " +
                           " <td width='600' colspan='3'> " + mailBody + " </td> " +
                       " </tr> " +

                       " </tbody> " +
                 " </table> " +
                 "</div> " +
                 "</body> " +
                 " </html>", null, "text/html");

                eposta.AlternateViews.Add(htmlView);

                eposta.IsBodyHtml = true;
                eposta.Subject = siteName + " " + mailTitle;

                NetworkCredential auth = new NetworkCredential(mailAddress, mailPassword);

                SmtpClient SMTP = new SmtpClient();
                SMTP.Host = mailServer;
                SMTP.Port = 587;

                SMTP.UseDefaultCredentials = false;
                SMTP.Credentials = auth;
                SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                SMTP.Send(eposta);

                returnValue = "başarılı";
            }
            catch (Exception ex)
            {
                returnValue = ex.ToString();
            }

            return returnValue;

        }


        public static string createMailBody(string titleData, string bodyData, string tableTileData = "")
        {
            string returnValue = "";

            if (tableTileData != "")
            {
                returnValue += "<tr style='background-color:#000;color:#fff;'> <td style='width:520px; text-align:left; padding:5px' colspan='3'> " + tableTileData + " </td></tr>";
            }

            string[] _titleData = titleData.Split(",".ToCharArray());
            string[] _bodyData = bodyData.Split(",".ToCharArray());

            for (int i = 0; i < _titleData.Length; i++)
            {
                for (int j = 0; j < _bodyData.Length; j++)
                {
                    string bgColor = "";
                    if (i % 2 == 0)
                    {
                        bgColor = "#dcdcdc";
                    }

                    returnValue += @"
                    <tr style='background-color:" + bgColor + "'> <td style='width:200px; text-align:left; padding:5px'> " + _titleData[i] + @" </td> 
                        <td style='width:10px; text-align:center; padding:5px'> : </td>
                        <td style='width:300px; text-align:left; padding:5px'> " + _bodyData[j] + @" </td>
                    </tr>";

                    i++;
                }
            }

            return returnValue;
        }
    }
}