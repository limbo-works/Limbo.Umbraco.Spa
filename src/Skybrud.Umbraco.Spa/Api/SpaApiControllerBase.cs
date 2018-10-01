using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Json.Resolvers;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Spa.Api {

    [JsonOnlyConfiguration]
    public abstract class SpaApiControllerBase : UmbracoApiController {

        /// <summary>
        /// Virtual method for handling redirects.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <param name="response">The response.</param>
        /// <returns><c>true</c> if a matching redirect was found; otherwise <c>false</c>.</returns>
        protected virtual bool HandleRedirects(SpaApiRequest request, out HttpResponseMessage response) {
            if (HandleSkybrudRedirect(request, out response)) return true;
            if (HandleUmbracoRedirect(request, out response)) return true;
            return false;
        }

        protected virtual bool HandleSkybrudRedirect(SpaApiRequest request, out HttpResponseMessage response) {

            response = null;

            // Look for a global Skybrud redirect
            RedirectItem redirect = RedirectsRepository.Current.GetRedirectByUrl(0, HttpUtility.UrlDecode(request.Url));

            // If nothing is found at this point, look for a site specific Skybrud redirect
            if (request.SiteId > 0 && redirect == null) {
                redirect = RedirectsRepository.Current.GetRedirectByUrl(request.SiteId, HttpUtility.UrlDecode(request.Url));
            }

            if (redirect == null) return false;

            // Return a redirect response based on the Skybrud redirect
            response = ReturnRedirect(request, redirect.LinkUrl, redirect.IsPermanent);
            return true;

        }
        
        protected virtual bool HandleUmbracoRedirect(SpaApiRequest request, out HttpResponseMessage response) {

            response = null;

            // Get a reference to Umbraco's redirect URL service
            IRedirectUrlService service = UmbracoContext.Application.Services.RedirectUrlService;

            // Look for a matching redirect
            IRedirectUrl umbRedirect = service.GetMostRecentRedirectUrl(request.SiteId + request.Url.TrimEnd('/'));
            if (umbRedirect == null) return false;

            // Get the destination page from the content cache
            IPublishedContent newContent = UmbracoContext.ContentCache.GetById(umbRedirect.ContentId);
            if (newContent == null) return false;

            // Send a redirect response if a page was found
            response = ReturnRedirect(request, newContent.Url, true);
            return true;

        }

        //protected virtual HttpResponseMessage ReturnRedirect(string url) {
        //    //return CreateSpaResponse(JsonMetaResponse.GetError(HttpStatusCode.MovedPermanently, "Page has moved"));
        //}

        protected virtual HttpResponseMessage ReturnRedirect(SpaApiRequest request, string destinationUrl) {
		    return ReturnRedirect(request, destinationUrl, HttpStatusCode.MovedPermanently);
	    }

        protected virtual HttpResponseMessage ReturnRedirect(SpaApiRequest request, string destinationUrl, bool permanent) {
            return ReturnRedirect(request, destinationUrl, permanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.TemporaryRedirect);
        }

        protected virtual HttpResponseMessage ReturnRedirect(SpaApiRequest request, string destinationUrl, HttpStatusCode statusCode) {

            // Initialize the "data" object for the response
            var body = new {
                url = destinationUrl
            };

            // Append scheme/protocol and host name if not already present
            if (destinationUrl.StartsWith("/")) {
                destinationUrl = $"{request.Protocol}://{request.HostName}{destinationUrl}";
            }

            // Generate the response
            return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, "Page has moved", body));

        }

        /// <summary>
        /// Returns an instance of <see cref="HttpResponseMessage"/> representing a server error.
        /// </summary>
        /// <returns>An instance of <see cref="HttpResponseMessage"/>.</returns>
        protected virtual HttpResponseMessage ReturnServerError() {
            return CreateSpaResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Internal server error"));
        }

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

        
        protected virtual IPublishedContent GetContentFromInput(IPublishedContent site, int nodeId, string url) {
            
            // Attempt to find the content item by it's route
            return UmbracoContext.ContentCache.GetByRoute(site.Id + url);

        }

        /// <summary>
        /// Virtual method called before the setup state of a SPA API request. Returns <c>true</c> if the method
        /// provides a response (through the <paramref name="response"/> parameter), otherwise <c>false</c>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <returns><c>true</c> if the method provides a response, otherwise <c>false</c>.</returns>
        protected virtual bool BeforeSetup(SpaApiRequest request, out HttpResponseMessage response) {
            response = null;
            return false;
        }

        protected virtual string Serialize(object data) {

            SpaGridJsonConverterBase gridConverter = new SpaGridJsonConverterBase();

            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings {
                ContractResolver = new SpaPublishedContentContractResolver(gridConverter)
            });

        }

        #region Abstract methods

        protected abstract int GetCultureIdFromUrl(SpaApiRequest request);

        #endregion

    }

}