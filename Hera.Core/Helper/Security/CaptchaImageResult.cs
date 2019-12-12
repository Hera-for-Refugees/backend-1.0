using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Mvc;

namespace Hera.Core.Helper.Security
{
    public class CaptchaImageResult : ActionResult
    {
        private string _type;
        public CaptchaImageResult(string type)
        {
            _type = type;
        }
        public string GetCaptchaString(int length)
        {
            int intZero = '1';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";
            Random random = new Random(System.DateTime.Now.Millisecond);
            while (intCount < length)
            {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ)))
                {
                    strCaptchaString = strCaptchaString + (char)intRandomNumber;
                    intCount = intCount + 1;
                }
            }
            return strCaptchaString.Replace("I", "X").Replace("O", "X");
        }
        public override void ExecuteResult(ControllerContext context)
        {
            Bitmap bmp = new Bitmap(130, 35);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(148, 148, 148));
            string randomString = GetCaptchaString(6);
            context.HttpContext.Session[this._type] = randomString;
            g.DrawString(randomString, new Font("Courier", 22), new SolidBrush(Color.Black), 2, 2);
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            bmp.Save(response.OutputStream, ImageFormat.Jpeg);
            bmp.Dispose();
        }
    }
}
