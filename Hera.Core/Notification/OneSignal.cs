using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Hera.Core.Notification
{
    public class OneSignal
    {

        public static string Send(string title, string message, List<string> deviceList, string type, string targetId)
        {
            var appId = ConfigurationManager.AppSettings["onesignalAppIdConsumer"];
            var apiKey = ConfigurationManager.AppSettings["onesginalApiKeyConsumer"];

            var requestObject = new
            {
                app_id = appId,
                headings = new { en = title },
                contents = new { en = message },
                data = new { notificationType = type, targetId = targetId },
                include_player_ids = deviceList.ToArray(),
                send_after = DateTime.Now.AddSeconds(1)
            };

            var payload = JsonConvert.SerializeObject(requestObject);

            try
            {
                var client = new RestClient("https://onesignal.com/api/");
                var request = new RestRequest("v1/notifications", Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json charset=utf-8");
                request.AddHeader("Authorization", "Basic " + apiKey);
                request.Parameters.Clear();
                request.AddParameter("application/json", payload, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return "Success => " + response.Content;
            }
            catch (Exception ex)
            {
                return "Fail => " + ex.Message;
            }
        }
    }
}
