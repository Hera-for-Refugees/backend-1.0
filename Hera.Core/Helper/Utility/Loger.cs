using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hera.Core.Helper.Utility
{
    public class Logger
    {
        public static void SetLog(bool isError, string message, LogType type)
        {
            var directoryPath = ConfigurationManager.AppSettings["logRoot"];
            if (string.IsNullOrEmpty(directoryPath))
                return;
            var directoryFullPath = HttpContext.Current.Server.MapPath(directoryPath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryFullPath);
            var currentDay = DateTime.Now.ToString("ddMMyyyy");
            var dayFullPath = Path.Combine(directoryFullPath, currentDay);
            if (!Directory.Exists(dayFullPath))
                Directory.CreateDirectory(dayFullPath);
            var typeFullPath = Path.Combine(dayFullPath, type.ToString());
            if (!Directory.Exists(typeFullPath))
                Directory.CreateDirectory(typeFullPath);

            var logFileName = $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}_{(isError ? "Error" : "Success")}.txt";
            var logFilePath = Path.Combine(typeFullPath, logFileName);
            try
            {
                using (var sw = new StreamWriter(logFilePath))
                {
                    sw.WriteLine(
                        $"Result:{(isError ? "Error" : "Success")}\nMessage:{message}\nDate:{DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public enum LogType
        {
            Mail, SMS, Notification
        }
    }
}
