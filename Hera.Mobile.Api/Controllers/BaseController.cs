using Hera.Core.Cache;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    public class BaseController : ApiController
    {
        public readonly Core.UnitOfWork.UnitOfWork unitOfWork;
        public readonly Models.MessageSource messageSource;
        public readonly InMemoryCache cacheService;
        public BaseController()
        {
            unitOfWork = new Core.UnitOfWork.UnitOfWork();
            messageSource = new Models.MessageSource();
            cacheService = new InMemoryCache();
        }
        public int GetLanguageId(string lang)
        {
            lang = lang.ToLower();
            var langList = cacheService.GetOrSet(Keys.SLanguage_List_ALL, 5, () =>
            {
                return unitOfWork.Repository<Data.Entity.SLanguage>().GetAll().ToList();
            });
            var currentLang = langList.Where(x => x.MobileCode == lang).FirstOrDefault();
            return currentLang == null ? 1 : currentLang.Id;

        }
        public string GetErrorTitle(string lang)
        {
            switch (lang)
            {
                case "tr": return "Hata !";
                case "ar": return "خطأ";
                default:
                    return "Error !";
            }
        }
    }
}
