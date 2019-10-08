using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.WebApi.Json.Meta;

namespace Skybrud.Umbraco.Spa.Api {

    public abstract partial class SpaControllerBase {

        #region Text

        public HttpResponseMessage CreateTextResponse(string text) {
            return CreateTextResponse(HttpStatusCode.OK, text, Encoding.UTF8);
        }

        public HttpResponseMessage CreateTextResponse(string text, Encoding encoding) {
            return CreateTextResponse(HttpStatusCode.OK, text, encoding);
        }

        public HttpResponseMessage CreateTextResponse(HttpStatusCode statusCode, string text, Encoding encoding) {
            return new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(text ?? string.Empty, encoding)
            };
        }

        #endregion

        #region HTML
        
        public HttpResponseMessage CreateHtmlResponse(string text) {
            return CreateHtmlResponse(HttpStatusCode.OK, text, Encoding.UTF8);
        }

        public HttpResponseMessage CreateHtmlResponse(string text, Encoding encoding) {
            return CreateHtmlResponse(HttpStatusCode.OK, text, encoding);
        }

        public HttpResponseMessage CreateHtmlResponse(HttpStatusCode statusCode, string text, Encoding encoding) {
            return new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(text ?? string.Empty, encoding, "text/html")
            };
        }
        
        protected HttpResponseMessage ReturnHtmlError(SpaRequest request, Exception ex) {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<style>");
            sb.AppendLine("body { font: 16px/1.4em Arial, sans-serif; background: #f1f1f1; color: #333; margin: 10px; }");
            sb.AppendLine("th { text-align: left; white-space: nowrap; min-width: 200px; padding-left: 10px; }");
            sb.AppendLine("h3 { margin-bottom: 5px; }");
            sb.AppendLine("th, td { vertical-align: top; padding: 3px; font-size: 12px; }");
            sb.AppendLine("</style>");

            sb.AppendLine("<h3>HTTP Request</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>Remote Address</th><td>" + request.HttpContext.Request.ServerVariables["REMOTE_ADDR"] + "</td></tr>");
            sb.AppendLine("<tr><th>User Agent</th><td>" + request.HttpContext.Request.UserAgent + "</td></tr>");
            sb.AppendLine("<tr><th>Accept Header</th><td>" + request.HttpContext.Request.Headers["Accept"] + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>SPA Options</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>Page ID</th><td>" + request.Arguments.PageId + "</td></tr>");
            sb.AppendLine("<tr><th>Site ID</th><td>" + request.Arguments.SiteId + "</td></tr>");
            sb.AppendLine("<tr><th>URL</th><td>" + request.Arguments.Url + "</td></tr>");
            sb.AppendLine("<tr><th>Query String</th><td>" + request.Arguments.QueryString + "</td></tr>");
            sb.AppendLine("<tr><th>Preview</th><td>" + request.Arguments.IsPreview + "</td></tr>");
            sb.AppendLine("<tr><th>Protocol</th><td>" + request.Arguments.Protocol + "</td></tr>");
            sb.AppendLine("<tr><th>Host name</th><td>" + request.Arguments.HostName + "</td></tr>");
            sb.AppendLine("<tr><th>Parts</th><td>" + string.Join(", ", from p in request.Arguments.Parts select p.ToString()) + "</td></tr>");
            sb.AppendLine("<tr><th>Nav levels</th><td>" + request.Arguments.NavLevels + "</td></tr>");
            sb.AppendLine("<tr><th>Nav context</th><td>" + request.Arguments.NavContext + "</td></tr>");
            sb.AppendLine("<tr><th>Cache key</th><td>" + request.Arguments.CacheKey + "</td></tr>");
            sb.AppendLine("<tr><th>Enable caching</th><td>" + request.Arguments.EnableCaching + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>Config</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>IsDebuggingEnabled</th><td>" + request.HttpContext.IsDebuggingEnabled + "</td></tr>");
            sb.AppendLine("<tr><th>IsCustomErrorEnabled</th><td>" + request.HttpContext.IsCustomErrorEnabled + "</td></tr>");
            sb.AppendLine("</table>");

            while (ex != null) {

                sb.AppendLine("<h3>Exception</h3>");
                sb.AppendLine("<table>\n");
                sb.AppendLine("<tr><th>Type</th><td>" + ex.GetType() + "</td></tr>");
                sb.AppendLine("<tr><th>Message</th><td>" + ex.Message + "</td></tr>");
                sb.AppendLine("<tr><th>Stack trace</th><td><pre>" + ex.StackTrace + "</pre></td></tr>");
                sb.AppendLine("</table>");

                ex = ex.InnerException;

            }

            return new HttpResponseMessage {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/html")
            };

        }

        #endregion

        #region SPA

        /// <summary>
        /// Returns a new JSON based instance of <see cref="HttpResponseMessage"/> for the specified
        /// <paramref name="data"/>. <paramref name="statusCode"/> is used as the status code in the response.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="data">The data to be serialized to JSON and returned as the response body.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage CreateSpaResponse(HttpStatusCode statusCode, object data) {
            return new HttpResponseMessage(statusCode) {
                Content = new StringContent(Serialize(data), Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Returns a new JSON based instance of <see cref="HttpResponseMessage"/> for the specified
        /// <paramref name="data"/>.
        /// 
        /// If <paramref name="data"/> is an instance of <see cref="JsonMetaResponse"/>, the status code will be
        /// inherited from the <see cref="JsonMetaData.Code"/> property. In any other cases, the status code will be
        /// <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <param name="data">The data to be serialized to JSON and returned as the response body.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage CreateSpaResponse(object data) {
            JsonMetaResponse meta = data as JsonMetaResponse;
            return meta != null ? CreateSpaResponse(meta.Meta.Code, meta) : CreateSpaResponse(HttpStatusCode.OK, data);
        }

        #endregion

        #region SPA/error

        /// <summary>
        /// Returns an instance of <see cref="HttpResponseMessage"/> representing a server error.
        /// </summary>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnError() {
            return CreateSpaResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Internal server error"));
        }

        protected virtual HttpResponseMessage ReturnError(HttpStatusCode code) {
            return CreateSpaResponse(JsonMetaResponse.GetError(code, null));
        }

        protected virtual HttpResponseMessage ReturnError(string message) {
            return CreateSpaResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, message));
        }

        protected virtual HttpResponseMessage ReturnError(HttpStatusCode code, string message) {
            return CreateSpaResponse(JsonMetaResponse.GetError(code, message));
        }

        protected virtual HttpResponseMessage ReturnError(HttpStatusCode code, string message, object body) {
            return CreateSpaResponse(JsonMetaResponse.GetError(code, message, body));
        }

        #endregion

        #region SPA/redirect

        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl) {
		    return ReturnRedirect(request, destinationUrl, HttpStatusCode.MovedPermanently);
	    }

        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl, bool permanent) {
            return ReturnRedirect(request, destinationUrl, permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.TemporaryRedirect);
        }

        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl, HttpStatusCode statusCode) {

            // Initialize the "data" object for the response
            var body = new {
                url = destinationUrl
            };

            // Generate the response
            return ReturnError(statusCode, "Page has moved", body);

        }

        #endregion

    }

}