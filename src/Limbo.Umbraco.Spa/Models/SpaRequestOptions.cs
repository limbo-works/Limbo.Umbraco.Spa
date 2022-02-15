using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Skybrud.Essentials.AspNetCore;
using Skybrud.Essentials.Enums;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing the options of a SPA request. The options are initialized from arguments in the SPA request,
    /// but as the request is processed, some properties may be updated to reflect the progress.
    /// </summary>
    public class SpaRequestOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the requested page. The value may be initialized from either the <c>pageId</c> or
        /// <c>nodeId</c> parameters in the query string.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the requested site. The value may be initialized from either the <c>siteId</c> or
        /// <c>appSiteId</c> parameters in the query string.
        /// </summary>
        public int SiteId { get; set; }
        
        /// <summary>
        /// Gets or sets the URL of the requested page.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the URI of the requested page.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets whether the current request is in preview mode.
        /// </summary>
        public bool IsPreview { get; set; }

        /// <summary>
        /// Gets a list of the requested <see cref="SpaApiPart"/> based on the <c>parts</c> query string parameter. If
        /// empty or not specified specified, all parts are assumed. 
        /// </summary>
        public List<SpaApiPart> Parts { get; set; }

        /// <summary>
        /// Gets or sets the protocol of the current request. If specified, this value will be based on the
        /// <c>appProtocol</c> query string parameter; otherwise it will come from the procotol of the current request.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the host name of the current request. If specified, this value will be based on the
        /// <c>appHost</c> query string parameter; otherwise it will come from the host name of the current request.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the port number of the inbound request.
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// Gets whether the inbound request is received via the default port number (eg. <c>80</c> for HTTP and <c>443</c> for HTTPS).
        /// </summary>
        public bool IsDefaultPort { get; set; }

        /// <summary>
        /// Gets the maximum amount of levels to be included in the <c>navigation</c> part. The value is based on the
        /// <c>navLevels</c> query string parameter, but defaults to <c>1</c> if not specified.
        /// </summary>
        public int NavLevels { get; set; }

        /// <summary>
        /// Gets or sets whether the <c>context</c> property should be part of the <see cref="SpaApiPart.Navigation"/> part.
        /// </summary>
        public bool NavContext { get; set; }

        /// <summary>
        /// Gets a unique key that identifies the current SPA API request. The key will be used for storing and retrieving the <c>data</c> part in the runtime cache.
        ///
        /// <strong>Notice:</strong> the default cache key does not take members into account. If your site has a members area with login, an identifier for the
        /// member currently logged in should be a part of the cache key.
        /// </summary>
        public virtual string CacheKey => $"{SpaConstants.CachePrefix}{PageId}-{SiteId}-{Url}-{IsPreview}-{string.Join(",", Parts ?? new List<SpaApiPart>())}-{Protocol}-{HostName}-{PortNumber}-{NavLevels}-{NavContext}";

        /// <summary>
        /// Gets a reference to the query string of the current SPA API request.
        /// </summary>
        public IQueryCollection QueryString { get; set; }

        /// <summary>
        /// Gets whether caching should be enabled for the current request.
        ///
        /// Caching is disabled by default when either of the following conditions are met:
        /// <ul>
        ///   <li>the site us running in compilation debug mode</li>
        ///   <li>the <c>cache</c> query string parameter is set to <c>false</c> or <c>0</c></li>
        ///   <li>the current request is in preview mode</li>
        /// </ul>
        /// </summary>
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Gets or sets whether the SPA should return a HTML response with exception details should part of the SPA or underlying logic fail.
        /// </summary>
        public bool ShowHtmlErrors { get; set; }

        /// <summary>
        /// Gets or sets the culture of the request.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets the remote address of the user.
        /// </summary>
        public string RemoteAddress { get; }

        /// <summary>
        /// Gets the user agent of the request.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Gets the accept types of the request.
        /// </summary>
        public string AcceptTypes { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">A SPA request.</param>
        /// <param name="helper">A current SPA request helper.</param>
        public SpaRequestOptions(SpaRequest request, SpaRequestHelper helper) : this(request.HttpContext, helper) { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">A HTTP context.</param>
        /// <param name="helper">A current SPA request helper.</param>
        public SpaRequestOptions(HttpContext context, SpaRequestHelper helper) {

            // Get a reference to the current request
            HttpRequest r = context.Request;

            // Get the URI of the current request
            Uri uri = r.GetUri();
            
            // Get the host name from the query
            string appHost = r.Query["appHost"];

            // Get the protocol from the query
            string appProtocol = r.Query["appProtocol"];

            // Use the current URL as fallback for "appHost" and "appProtocol"
            HostName = string.IsNullOrWhiteSpace(appHost) ? uri.Host : appHost;
            Protocol = string.IsNullOrWhiteSpace(appProtocol) ? uri.Scheme : appProtocol;
            PortNumber = uri.Port;
            IsDefaultPort = uri.IsDefaultPort;

            // Parse the "navLevels" and "navContext" parameters from the query string
            NavLevels = r.Query.GetInt32("navLevels", 1);
            NavContext = r.Query.GetBoolean("navContext");

            // Parse the requests "parts"
            Parts = GetParts(r.Query["parts"]);

            // Get the URL and URI of the requested page
            Url = r.Query["url"];
            Uri = new Uri($"{Protocol}://{HostName}{Url}");

            // Parse the "siteId" and "pageId" parameters
            SiteId = r.Query.GetInt32("siteId", -1);
            PageId = r.Query.GetInt32("pageId", -1);

            QueryString = r.Query;

            Culture = r.Query["culture"];

            // Determine whether the current request is in debug mode
            if (helper.TryGetPreviewId(Url, out int previewId)) {
                PageId = previewId;
                IsPreview = true;
            }

            RemoteAddress = r.GetRemoteAddress() ?? string.Empty;
            UserAgent = r.GetUserAgent() ?? string.Empty;
            AcceptTypes = r.GetAcceptTypes() ?? string.Empty;

            // Determine whether caching should be enabled
            EnableCaching = helper.Environment.IsDevelopment() == false && r.Query.GetBoolean("cache", true) && IsPreview == false;

            ShowHtmlErrors = helper.Environment.IsDevelopment()/* && context.IsCustomErrorEnabled == false*/ && AcceptTypes.Contains("text/html");

        }

        #endregion

        #region Member methods

        /// <summary>
        /// Converts the specified string of <paramref name="parts"/> to <see cref="List{SpaApiPart}"/>.
        /// </summary>
        /// <param name="parts">The string with the parts.</param>
        /// <returns>An an instance of <see cref="List{SpaApiPart}"/> containing each <see cref="SpaApiPart"/> specified in <paramref name="parts"/>.</returns>
        private static List<SpaApiPart> GetParts(string parts = "") {
            
            // No parts means all parts
            if (string.IsNullOrWhiteSpace(parts)) return new List<SpaApiPart> { SpaApiPart.Content, SpaApiPart.Navigation, SpaApiPart.Site };

            List<SpaApiPart> temp = new List<SpaApiPart>();
            foreach (string item in parts.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                if (EnumUtils.TryParseEnum(item, out SpaApiPart part)) temp.Add(part);
            }

            return temp;

        }

        #endregion

    }

}