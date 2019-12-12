using Hera.Core.Cache;
using Hera.Core.UnitOfWork;
using System.Linq;

namespace Hera.Mobile.Api.Models
{
    public class MessageSource
    {
        public UnitOfWork unitOfWork;
        public readonly InMemoryCache cacheService;
        public MessageSource()
        {
            unitOfWork = new UnitOfWork();
            cacheService = new InMemoryCache();
        }
        public string GetServiceMessage(string screen, string label, string lang)
        {
            lang = lang.ToLower();
            var langList = cacheService.GetOrSet(Keys.SLanguage_List_ALL, 5, () =>
            {
                return unitOfWork.Repository<Data.Entity.SLanguage>().GetAll().ToList();
            });

            var langId = 1;
            var currentLang = langList.Where(x => x.MobileCode == lang).FirstOrDefault();

            langId = currentLang == null ? 1 : currentLang.Id;
            var screenData = unitOfWork.Repository<Data.Entity.App_Screen>().GetBy(x => x.Name == screen).FirstOrDefault();
            if (screenData == null)
            {
                return "MessageByTranslation";
            }
            else
            {
                var text = screenData.App_Screen_Text.Where(x => x.Label == label && x.LanguageId == langId).FirstOrDefault();
                if (text == null)
                {
                    return "MessageByTranslation";
                }
                else
                {
                    return text.Translation;
                }
            }
        }
    }
}