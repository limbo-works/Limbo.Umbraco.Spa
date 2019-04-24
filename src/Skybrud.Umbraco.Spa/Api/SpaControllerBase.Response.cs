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