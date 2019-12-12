using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Hera.Core.Base
{
    public class BaseController : Controller
    {
        public UnitOfWork.UnitOfWork unitOfWork;
        public readonly Cache.InMemoryCache cacheService;
        public CustomResult Result { get; set; }
        public BaseController()
        {
            unitOfWork = new UnitOfWork.UnitOfWork();
            cacheService = new Cache.InMemoryCache();
            this.Result = new CustomResult();
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;
            HttpCookie cultureCookie = Request.Cookies["_wdtCulture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
            cultureName = Core.Helper.Culture.CultureHelper.GetImplementedCulture(cultureName);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            return base.BeginExecuteCore(callback, state);
        }

    }

    public class CustomResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public dynamic Result { get; set; }
    }
}
