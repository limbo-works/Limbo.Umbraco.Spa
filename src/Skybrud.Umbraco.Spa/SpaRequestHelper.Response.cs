using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.WebApi.Json.Meta;

namespace Skybrud.Umbraco.Spa {

    public abstract partial class SpaRequestHelper {

        #region Text

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text representing the response body.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateTextResponse(string text) {
            return CreateTextResponse(HttpStatusCode.OK, text, Encoding.UTF8);
        }

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="text"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="text">The text representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateTextResponse(string text, Encoding encoding) {
            return CreateTextResponse(HttpStatusCode.OK, text, encoding);
        }

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="statusCode"/>, <paramref name="text"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="text">The text representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateTextResponse(HttpStatusCode statusCode, string text, Encoding encoding) {
            return new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(text ?? string.Empty, encoding)
            };
        }

        #endregion

        #region HTML

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="html"/>.
        /// </summary>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateHtmlResponse(string html) {
            return CreateHtmlResponse(HttpStatusCode.OK, html, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="html"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateHtmlResponse(string html, Encoding encoding) {
            return CreateHtmlResponse(HttpStatusCode.OK, html, encoding);
        }

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="statusCode"/>, <paramref name="html"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        public virtual HttpResponseMessage CreateHtmlResponse(HttpStatusCode statusCode, string html, Encoding encoding) {
            return new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(html ?? string.Empty, encoding, "text/html")
            };
        }

        /// <summary>
        /// Creates a new HTML based error response for the specified <paramref name="exception"/>.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="exception">The exception the response should be about.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnHtmlError(SpaRequest request, Exception exception) {

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
            if (request.Arguments == null) {
                sb.AppendLine("<table><tr><td><em><code>request.Arguments</code> has not yet been initialized.</em></td></tr></table>");
            } else {
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
            }

            sb.AppendLine("<h3>Config</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>IsDebuggingEnabled</th><td>" + request.HttpContext.IsDebuggingEnabled + "</td></tr>");
            sb.AppendLine("<tr><th>IsCustomErrorEnabled</th><td>" + request.HttpContext.IsCustomErrorEnabled + "</td></tr>");
            sb.AppendLine("</table>");

            while (exception != null) {

                sb.AppendLine("<h3>Exception</h3>");
                sb.AppendLine("<table>\n");
                sb.AppendLine("<tr><th>Type</th><td>" + exception.GetType() + "</td></tr>");
                sb.AppendLine("<tr><th>Message</th><td><pre>" + exception.Message + "</pre></td></tr>");
                sb.AppendLine("<tr><th>Stack trace</th><td><pre>" + exception.StackTrace + "</pre></td></tr>");
                sb.AppendLine("</table>");

                exception = exception.InnerException;

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
            return data is JsonMetaResponse meta ? CreateSpaResponse(meta.Meta.Code, meta) : CreateSpaResponse(HttpStatusCode.OK, data);
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

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnError(HttpStatusCode statusCode) {
            return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, null));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message of the error response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnError(string message) {
            return CreateSpaResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, message));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="message">The message of the error response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnError(HttpStatusCode statusCode, string message) {
            return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, message));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/>, <paramref name="message"/> and <paramref name="data"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="message">The message of the error response.</param>
        /// <param name="data">The value of the <c>data</c> property in the JSON response.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnError(HttpStatusCode statusCode, string message, object data) {
            return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, message, data));
        }

        #endregion

        #region SPA/redirect

        /// <summary>
        /// Returns a JSON response for a permanent SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl) {
		    return ReturnRedirect(request, destinationUrl, HttpStatusCode.MovedPermanently);
	    }

        /// <summary>
        /// Returns a JSON response for a SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <param name="permanent">Whether the redirect is permanent (<see cref="HttpStatusCode.MovedPermanently"/>) or temporary (<see cref="HttpStatusCode.TemporaryRedirect"/>).</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl, bool permanent) {
            return ReturnRedirect(request, destinationUrl, permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.TemporaryRedirect);
        }

        /// <summary>
        /// Returns a JSON response for a SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <param name="statusCode">The status code of the response - eg. <see cref="HttpStatusCode.MovedPermanently"/>.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnRedirect(SpaRequest request, string destinationUrl, HttpStatusCode statusCode) {
            
            // Initialize the "data" object for the response
            var data = new {
                url = destinationUrl,
                permanent = statusCode == HttpStatusCode.MovedPermanently
            };
            
            // Initialize the response body (including the correct status code)
            JsonMetaResponse body = JsonMetaResponse.GetError(statusCode, "Page has moved", data);

            // Generate the response (using "418 I'm a teapot" as the status code to support older browsers and systems)
            return CreateSpaResponse(SpaConstants.Teapot, body);

        }

        #endregion

    }

}