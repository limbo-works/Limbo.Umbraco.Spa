using System.Web.Http.Controllers;
using System.Web.Http.Filters;

// ReSharper disable InconsistentNaming

namespace Skybrud.Umbraco.Spa.Attributes {

    /// <summary>
    /// Attribute used on WebAPI controllers for setting the <c>Access-Control-Allow-Origin</c> header to <c>*</c>.
    /// </summary>
    public class AccessControlAllowOriginAttribute : ActionFilterAttribute {
        
        /// <inheritdoc />
        public override void OnActionExecuting(HttpActionContext actionContext) {
            actionContext.Request.Headers.Add("Access-Control-Allow-Origin", "*");
        }

    }

}