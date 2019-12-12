using System.Linq;

namespace Hera.Mobile.Api.Models
{
    public class App
    {
        /// <summary>
        /// Default language mobile code for DB
        /// </summary>
        public const string DEFAULT_LANGUAGE_MOBILE_CODE = "en";
        /// <summary>
        /// Base media path
        /// </summary>
        public const string MEDIA_ROOT_DOMAIN = "http://hera.wdtajans.com";
        /// <summary>
        /// Default member logo path
        /// </summary>
        public const string DEFAULT_MEMBER_LOGO = "/Media/Member/Logo/memberDefault.png";
        /// <summary>
        /// Default datetime format for toString method
        /// </summary>
        public const string DEFAULT_DATETIME_FORMAT = "dd/MM/yyyy";

        /// <summary>
        /// Getting LanguageId from database by parameter
        /// </summary>
        /// <param name="mobileCode">request device mobile code (tr,en,de vb..)</param>
        /// <returns></returns>
        public static int GetLanguageId(string mobileCode)
        {
            using (var unit = new Core.UnitOfWork.UnitOfWork())
            {
                var lang = unit.Repository<Data.Entity.SLanguage>().GetBy(x => x.MobileCode == mobileCode).FirstOrDefault();
                if (lang == null)
                    lang = unit.Repository<Data.Entity.SLanguage>().GetBy(x => x.MobileCode == DEFAULT_LANGUAGE_MOBILE_CODE).FirstOrDefault();
                return lang.Id;
            }
        }

    }
}