using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Hera.Core.Mail
{
    public class MailSender
    {
        public static string MailHost { get { return ConfigurationManager.AppSettings["mailSenderHost"]; } }
        public static string MailPort { get { return ConfigurationManager.AppSettings["mailSenderPort"]; } }
        public static string Mail { get { return ConfigurationManager.AppSettings["mailSenderEmail"]; } }
        public static string MailPassword { get { return ConfigurationManager.AppSettings["mailSenderPassword"]; } }
        public static List<string> MailToList { get { return ConfigurationManager.AppSettings["mailSenderToList"].Split(';').ToList(); } }


        #region ForgetPassword
        public static void SendForgetPassword(string mailAddress, string memberNameSurname, string newPassword)
        {
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("~/Mail/MobileForgetPassword.html")))
            {
                var mailBody = sr.ReadToEnd();
                mailBody = mailBody.Replace("xNameSurnamex", memberNameSurname);
                mailBody = mailBody.Replace("xNewPassx", newPassword);
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);
                avHtml.LinkedResources.Add(new LinkedResource(HttpContext.Current.Server.MapPath("~/Mail/appLogo.png"), MediaTypeNames.Image.Jpeg) { ContentId = "HeaderLogo" });
                var mailList = new List<string> { mailAddress };
                SendMail("App Mobil Uygulama Şifremi Unuttum ?", mailList, avHtml);
            }
        }
        #endregion


        static void SendMail(string subject, List<string> to, AlternateView htmlView)
        {
            try
            {
                using (var smtpClient = new SmtpClient(MailHost, Convert.ToInt32(MailPort)))
                {
                    using (var mailMsg = new MailMessage())
                    {
                        mailMsg.From = new MailAddress(Mail, "Hera App");
                        to.ForEach(x => mailMsg.To.Add(x));
                        smtpClient.UseDefaultCredentials = false;
                        var SMTPUserInfo = new NetworkCredential(Mail, MailPassword);
                        smtpClient.Credentials = SMTPUserInfo;
                        mailMsg.Subject = subject;
                        mailMsg.AlternateViews.Add(htmlView);
                        smtpClient.Send(mailMsg);
                        SetLog(false, "Send Mail Success !");
                    }
                }
            }
            catch (Exception ex)
            {
                SetLog(true, ex.Message);
            }
        }
        static void SetLog(bool isError, string message)
        {
            var logFilePath = HttpContext.Current.Server.MapPath("~/App_Data/SendMail_Log.txt");
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath))
                {
                    sw.WriteLine(string.Format("Result:{0} => Message:{1} => Date:{2}", isError ? "Error" : "Success", message, DateTime.Now.ToString()));
                    sw.Flush();
                    sw.Close();
                }
            }
            finally { }
        }
    }
}
