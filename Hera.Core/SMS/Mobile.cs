using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace Hera.Core.SMS
{
    public class Mobile
    {
        public string username { get; set; }
        public string password { get; set; }
        public string source_addr { get; set; }
        public string datacoding { get; set; }
        public string valid_for { get; set; }
        public Message[] messages { get; set; }

        public static void SendSms(string targetNumber, string message)
        {
            if (targetNumber.StartsWith("0"))
                targetNumber = targetNumber.Remove(0, 1);
            var host = ConfigurationManager.AppSettings["smsHost"];
            var requestObject = new Mobile
            {
                username = ConfigurationManager.AppSettings["smsUsername"],
                password = ConfigurationManager.AppSettings["smsPassword"],
                source_addr = ConfigurationManager.AppSettings["smsSubject"],
                datacoding = "0",
                valid_for = "00:03",
                messages = new Message[] { new Message { dest = targetNumber, msg = message } }
            };
            var payload = JsonConvert.SerializeObject(requestObject);

            try
            {
                var client = new RestClient(host);
                var request = new RestRequest("v2/send.json", Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", payload, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                SetLog(false, result + " => " + targetNumber + " => " + message + " => " + host + "=>" + payload);
            }
            catch (Exception ex)
            {
                SetLog(true, ex.Message);
            }
        }

        static void SetLog(bool isError, string message)
        {
            try
            {
                var logFilePath = HttpContext.Current.Server.MapPath("~/App_Data/SMS_Log.txt");
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
