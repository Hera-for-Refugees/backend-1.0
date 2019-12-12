using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Hera.Core.Helper.Media
{
    public enum PictureType
    {
        Member_Profile,
        Member_Health_Record
    }

    public class Picture
    {
        private static readonly Regex DataUriPattern = new Regex(@"^data\:(?<type>image\/(png|tiff|jpg|gif));base64,(?<data>[A-Z0-9\+\/\=]+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private Picture(string mimeType, byte[] rawData)
        {
            MimeType = mimeType;
            RawData = rawData;
        }

        public string MimeType { get; }
        public byte[] RawData { get; }

        public Image Image => Image.FromStream(new MemoryStream(RawData));

        public static Picture TryParse(string dataUri)
        {
            if (string.IsNullOrWhiteSpace(dataUri)) return null;

            Match match = DataUriPattern.Match(dataUri);
            if (!match.Success) return null;

            string mimeType = match.Groups["type"].Value;
            string base64Data = match.Groups["data"].Value;

            try
            {
                byte[] rawData = Convert.FromBase64String(base64Data);
                return rawData.Length == 0 ? null : new Picture(mimeType, rawData);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static string CreateFromBase64(string base64Image, PictureType type)
        {
            //var mediaRoot = ConfigurationManager.AppSettings["mediaRoot"];
            var extension = "jpg";
            if (base64Image.Contains("/png"))
                extension = "png";
            var result = "";
            switch (type)
            {
                case PictureType.Member_Profile:
                    result = string.Format("~/Media/Member/{0}.{1}", Guid.NewGuid(), extension);
                    break;
                case PictureType.Member_Health_Record:
                    result = string.Format("~/Media/Health/{0}.{1}", Guid.NewGuid(), extension);
                    break;
                default:
                    break;
            }
            byte[] imgBytes = Convert.FromBase64String(base64Image.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;", "").Replace("base64,", ""));
            using (var ms = new MemoryStream(imgBytes))
            {
                var img = Image.FromStream(ms);
                img.Save(HttpContext.Current.Server.MapPath(result));
            }
            return result;
        }
    }
}
