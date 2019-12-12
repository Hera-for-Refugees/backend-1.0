using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hera.Core.Base
{
    [Authorize]
    public class ManageController : BaseController
    {
        public string SourceTable { get; set; }
        public int SourceTableId { get; set; }
        public List<SelectListItem> LanguageList
        {
            get
            {
                return cacheService.GetOrSet(Core.Cache.Keys.SLanguage_List_ALL, 5, () =>
                {
                    return unitOfWork.Repository<Data.Entity.SLanguage>().GetAll().ToList().ConvertToSelectList("Name", "Id");
                });
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.UnReadMessage = 0;
            if (!User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "User"));
            }
            else
            {
                var systemUser = unitOfWork.Repository<Data.Entity.SUser>().GetBy(x => x.Email == User.Identity.Name).FirstOrDefault();
                if (systemUser == null)
                {
                    filterContext.Result = new RedirectResult(Url.Action("Login", "User"));
                }
                else
                {
                    ViewData["LoggedUser"] = systemUser;
                }
            }
        }

    }
}
