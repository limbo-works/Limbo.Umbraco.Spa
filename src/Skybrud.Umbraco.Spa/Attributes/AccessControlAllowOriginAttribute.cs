using System.Web.Http.Filters;

// ReSharper disable InconsistentNaming

namespace Skybrud.Umbraco.Spa.Attributes {

    /// <summary>
    /// Attribute used on WebAPI controllers for setting the <c>Access-Control-Allow-Origin</c> HTTP header to <see cref="Value"/>.
    /// </summary>
    public class AccessControlAllowOriginAttribute : ActionFilterAttribute {

        /// <summary>
        /// Gets the value to be set for the HTTP header.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new attribute with <c>*</c> as the value.
        /// </summary>
        public AccessControlAllowOriginAttribute() {
            Value = "*";
        }

        /// <summary>
        /// Initializes a new instance with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be set for the HTTP header.</param>
        public AccessControlAllowOriginAttribute(string value) {
            Value = value;
        }

        /// <inheritdoc />
        public override void OnActionExecuted(HttpActionExecutedContext context) {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

    }

}