using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Skybrud.Umbraco.Spa.Models;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Spa.Attributes {

    public class SpaMicroCacheAttribute : ActionFilterAttribute {
        
        public SpaRequestOptions Options { get; private set; }

        public string CacheKey { get; private set; }

        public override void OnActionExecuting(HttpActionContext context) {

            Options = new SpaRequestOptions(new HttpContextWrapper(HttpContext.Current));

            if (Options.EnableCaching == false) return;

            CacheKey = Options.CacheKey;

            IAppPolicyCache cache = Current.AppCaches.RuntimeCache;

            HttpResponseMessage cachedModel = cache.Get(CacheKey) as HttpResponseMessage;
            if (cachedModel == null) return;

            context.Response = cachedModel;

        }

        public override void OnActionExecuted(HttpActionExecutedContext context) {

            if (Options.EnableCaching == false) return;

            IAppPolicyCache cache = Current.AppCaches.RuntimeCache;

            cache.InsertCacheItem(Options.CacheKey, () => context.Response, TimeSpan.FromSeconds(60));

        }

    }

}