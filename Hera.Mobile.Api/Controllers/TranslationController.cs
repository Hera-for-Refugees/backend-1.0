using Hera.Mobile.Api.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    [Authorize]
    public class TranslationController : BaseController
    {
        public Response<List<Models.Translation.TranslationItem>> List(string Lang)
        {
            var response = new Response<List<Models.Translation.TranslationItem>>
            {
                Result = new List<Models.Translation.TranslationItem>()
            };
            var language = unitOfWork.Repository<Data.Entity.SLanguage>().GetBy(x => x.MobileCode == Lang).FirstOrDefault();
            if (language == null)
                language = unitOfWork.Repository<Data.Entity.SLanguage>().GetBy(x => x.MobileCode == "tr").FirstOrDefault();


            var dataList = unitOfWork.Repository<Data.Entity.sp_Mobile_Translation_List_Result>().ExecWtihSP("EXEC sp_Mobile_Translation_List @LanguageId",
                new SqlParameter("@LanguageId", language.Id)).ToList();

            var screenList = dataList.Select(x => x.Screen).Distinct().ToList();
            foreach (var item in screenList)
            {
                var screen = new Models.Translation.TranslationItem
                {
                    Screen = item,
                    Translation = new Dictionary<string, string>()
                };
                foreach (var label in dataList.Where(x => x.Screen == item))
                {
                    screen.Translation.Add(label.Label, label.Translation);
                }
                response.Result.Add(screen);
            }

            return response;
        }
    }
}
