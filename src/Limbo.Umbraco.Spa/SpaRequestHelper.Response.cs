using System;
using System.Linq;
using System.Net;
using System.Text;
using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Skybrud.Essentials.AspNetCore;
using Skybrud.Essentials.Reflection;

namespace Limbo.Umbraco.Spa {

    public abstract partial class SpaRequestHelper {

        #region Text

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text representing the response body.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateTextResponse(string text) {
            return CreateTextResponse(HttpStatusCode.OK, text, Encoding.UTF8);
        }

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="text"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="text">The text representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateTextResponse(string text, Encoding encoding) {
            return CreateTextResponse(HttpStatusCode.OK, text, encoding);
        }

        /// <summary>
        /// Initializes a new text response from the specified <paramref name="statusCode"/>, <paramref name="text"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="text">The text representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateTextResponse(HttpStatusCode statusCode, string text, Encoding encoding) {
            return new ContentResult {
                StatusCode = (int) HttpStatusCode.InternalServerError,
                ContentType = "text/plain",
                Content = text ?? string.Empty
            };
        }

        #endregion

        #region HTML

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="html"/>.
        /// </summary>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateHtmlResponse(string html) {
            return CreateHtmlResponse(HttpStatusCode.OK, html, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="html"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateHtmlResponse(string html, Encoding encoding) {
            return CreateHtmlResponse(HttpStatusCode.OK, html, encoding);
        }

        /// <summary>
        /// Creates a new HTMl based response from the specified <paramref name="statusCode"/>, <paramref name="html"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="html">The HTML value representing the response body.</param>
        /// <param name="encoding">The encoding to be used for the response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        public virtual ActionResult CreateHtmlResponse(HttpStatusCode statusCode, string html, Encoding encoding) {
            return new ContentResult {
                StatusCode = (int) HttpStatusCode.InternalServerError,
                ContentType = "text/html",
                Content = html ?? string.Empty
            };
        }

        /// <summary>
        /// Creates a new HTML based error response for the specified <paramref name="exception"/>.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="exception">The exception the response should be about.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnHtmlError(SpaRequest request, Exception exception) {

            StringBuilder sb = new();

            sb.AppendLine("<style>");
            sb.AppendLine("body { font: 16px/1.4em Arial, sans-serif; background: #f1f1f1; color: #333; margin: 10px; }");
            sb.AppendLine("th { text-align: left; white-space: nowrap; min-width: 200px; padding-left: 10px; }");
            sb.AppendLine("h3 { margin-bottom: 5px; }");
            sb.AppendLine("th, td { vertical-align: top; padding: 3px; font-size: 12px; }");
            sb.AppendLine("</style>");

            if (Environment.IsDevelopment()) {
                sb.AppendLine("<h3>Limbo SPA</h3>");
                sb.AppendLine("<table>\n");
                sb.AppendLine("<tr><th>Version</th><td>" + ReflectionUtils.GetInformationalVersion(typeof(SpaRequest).Assembly) + "</td></tr>");
                sb.AppendLine("</table>");
            }

            sb.AppendLine("<h3>HTTP Request</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>Remote Address</th><td>" + request.Arguments.RemoteAddress + "</td></tr>");
            sb.AppendLine("<tr><th>User Agent</th><td>" + request.Arguments.UserAgent + "</td></tr>");
            sb.AppendLine("<tr><th>Accept Header</th><td>" + request.Arguments.AcceptTypes + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>SPA Options</h3>");
            if (request.Arguments == null) {
                sb.AppendLine("<table><tr><td><em><code>request.Arguments</code> has not yet been initialized.</em></td></tr></table>");
            } else {
                sb.AppendLine("<table>\n");
                sb.AppendLine("<tr><th>Page ID</th><td>" + request.Arguments.PageId + "</td></tr>");
                sb.AppendLine("<tr><th>Site ID</th><td>" + request.Arguments.SiteId + "</td></tr>");
                sb.AppendLine("<tr><th>URL</th><td>" + request.Arguments.Url + "</td></tr>");
                sb.AppendLine("<tr><th>Query String</th><td>" + request.Arguments.QueryString.ToUrlEncodedString() + "</td></tr>");
                sb.AppendLine("<tr><th>Preview</th><td>" + request.Arguments.IsPreview + "</td></tr>");
                sb.AppendLine("<tr><th>Protocol</th><td>" + request.Arguments.Protocol + "</td></tr>");
                sb.AppendLine("<tr><th>Host name</th><td>" + request.Arguments.HostName + "</td></tr>");
                sb.AppendLine("<tr><th>Port</th><td>" + request.Arguments.Uri.Port + "</td></tr>");
                sb.AppendLine("<tr><th>Parts</th><td>" + string.Join(", ", from p in request.Arguments.Parts select p.ToString()) + "</td></tr>");
                sb.AppendLine("<tr><th>Nav levels</th><td>" + request.Arguments.NavLevels + "</td></tr>");
                sb.AppendLine("<tr><th>Nav context</th><td>" + request.Arguments.NavContext + "</td></tr>");
                sb.AppendLine("<tr><th>Cache key</th><td>" + request.Arguments.CacheKey + "</td></tr>");
                sb.AppendLine("<tr><th>Enable caching</th><td>" + request.Arguments.EnableCaching + "</td></tr>");
                sb.AppendLine("</table>");
            }

            sb.AppendLine("<h3>Config</h3>");
            sb.AppendLine("<table>\n");
            sb.AppendLine("<tr><th>IsDevelopment</th><td>" + Environment.IsDevelopment() + "</td></tr>");
            //sb.AppendLine("<tr><th>IsCustomErrorEnabled</th><td>" + request.HttpContext.IsCustomErrorEnabled + "</td></tr>");
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

            // Create a new response
            return CreateHtmlResponse(HttpStatusCode.InternalServerError, sb.ToString(), Encoding.UTF8);

        }

        #endregion

        #region SPA

        /// <summary>
        /// Returns a new JSON based instance of <see cref="ActionResult"/> for the specified
        /// <paramref name="data"/>. <paramref name="statusCode"/> is used as the status code in the response.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="data">The data to be serialized to JSON and returned as the response body.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult CreateSpaResponse(HttpStatusCode statusCode, object data) {

            // Ensure that the status code is set on the data model so the status code is kept when caching the data model
            if (data is SpaDataModel dataModel) dataModel.Meta.StatusCode = statusCode;

            // Overwrite the status code to make the frontenders happy
            if (OverwriteStatusCodes) {
                switch (statusCode) {
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.TemporaryRedirect:
                    case HttpStatusCode.MovedPermanently:
                        statusCode = HttpStatusCode.OK;
                        break;
                }
            }

            // Create a new response
            return new ContentResult {
                StatusCode = (int) statusCode,
                ContentType = "application/json",
                Content = Serialize(data)
            };

        }

        /// <summary>
        /// Returns a new JSON based instance of <see cref="ActionResult"/> for the specified
        /// <paramref name="data"/>.
        ///
        /// If <paramref name="data"/> is an instance of <see cref="SpaResponseModel"/>, the status code will be
        /// inherited from the <see cref="SpaMetaData.Code"/> property. In any other cases, the status code will be
        /// <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <param name="data">The data to be serialized to JSON and returned as the response body.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult CreateSpaResponse(object data) {
            return data is SpaResponseModel meta ? CreateSpaResponse(meta.Meta.Code, meta) : CreateSpaResponse(HttpStatusCode.OK, data);
        }

        #endregion

        #region SPA/error

        /// <summary>
        /// Returns an instance of <see cref="ActionResult"/> representing a server error.
        /// </summary>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnError() {
            return CreateSpaResponse(SpaResponseModel.GetError(HttpStatusCode.InternalServerError, "Internal server error"));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnError(HttpStatusCode statusCode) {
            return CreateSpaResponse(SpaResponseModel.GetError(statusCode, null));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message of the error response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnError(string message) {
            return CreateSpaResponse(SpaResponseModel.GetError(HttpStatusCode.InternalServerError, message));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="message">The message of the error response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnError(HttpStatusCode statusCode, string message) {
            return CreateSpaResponse(SpaResponseModel.GetError(statusCode, message));
        }

        /// <summary>
        /// Returns a new JSON based error response with the specified <paramref name="message"/>, <paramref name="message"/> and <paramref name="data"/>.
        /// </summary>
        /// <param name="statusCode">The status code to be used for the response.</param>
        /// <param name="message">The message of the error response.</param>
        /// <param name="data">The value of the <c>data</c> property in the JSON response.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnError(HttpStatusCode statusCode, string message, object data) {
            return CreateSpaResponse(SpaResponseModel.GetError(statusCode, message, data));
        }

        #endregion

        #region SPA/redirect

        /// <summary>
        /// Returns a JSON response for a permanent SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnRedirect(SpaRequest request, string destinationUrl) {
            return ReturnRedirect(request, destinationUrl, HttpStatusCode.MovedPermanently);
        }

        /// <summary>
        /// Returns a JSON response for a SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <param name="permanent">Whether the redirect is permanent (<see cref="HttpStatusCode.MovedPermanently"/>) or temporary (<see cref="HttpStatusCode.TemporaryRedirect"/>).</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnRedirect(SpaRequest request, string destinationUrl, bool permanent) {
            return ReturnRedirect(request, destinationUrl, permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.TemporaryRedirect);
        }

        /// <summary>
        /// Returns a JSON response for a SPA redirect.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="destinationUrl">The destination URL of the redirect.</param>
        /// <param name="statusCode">The status code of the response - eg. <see cref="HttpStatusCode.MovedPermanently"/>.</param>
        /// <returns>An instance of <see cref="ActionResult"/>.</returns>
        protected virtual ActionResult ReturnRedirect(SpaRequest request, string destinationUrl, HttpStatusCode statusCode) {

            // Initialize the "data" object for the response
            var data = new {
                url = destinationUrl,
                permanent = statusCode == HttpStatusCode.MovedPermanently
            };

            // Initialize the response body (including the correct status code)
            SpaResponseModel body = SpaResponseModel.GetError(statusCode, "Page has moved", data);

            // Overwrite the status code to make the frontenders happy
            statusCode = Configuration.OverwriteStatusCodes ? HttpStatusCode.OK : SpaConstants.Teapot;

            // Generate the response
            return CreateSpaResponse(statusCode, body);

        }

        #endregion

    }

}