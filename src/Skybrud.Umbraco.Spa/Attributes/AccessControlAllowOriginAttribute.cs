using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Skybrud.Umbraco.Spa.Attributes {

    public class AccessControlAllowOriginAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(HttpActionContext actionContext) {
            actionContext.Request.Headers.Add("Access-Control-Allow-Origin", "*");
        }

    }

}