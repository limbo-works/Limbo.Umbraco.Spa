using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Spa.Extensions;

namespace Skybrud.Umbraco.Spa.Models {

    /// <summary>
    // Class representing the options of a SPA request. The options are initialized from arguments in the SPA request,
    // but as the request is processed, some properties may be updated to reflect the progress.
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
        /// Gets the maximum amount of levels to be included in the <c>navigation</c> part. The value is based on the
        /// <c>navLevels</c> query string parameter, but defaults to <c>1</c> if not specified.
        /// </summary>
        public int NavLevels { get; set; }

        public bool NavContext { get; set; }

        /// <summary>
        /// Gets a unique key that identifies the current SPA API request. The key will be used for storing and retrieving the <c>data</c> part in the runtime cache.
        ///
        /// <strong>Notice:</strong> the default cache key does not take members into account. If your site has a members area with login, an identifier for the
        /// member currently logged in should be a part of the cache key.
        /// </summary>
        public virtual string CacheKey => $"SpaMicroCache-{PageId}-{SiteId}-{Url}-{IsPreview}-{string.Join(",", Parts ?? new List<SpaApiPart>())}-{Protocol}-{HostName}-{NavLevels}-{NavContext}";

        /// <summary>
        /// Gets a reference to the query string of the current SPA API request.
        /// </summary>
        public NameValueCollection QueryString { get; set; }

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
        ///
        /// By default HTML errors will be shown when the following criteria are met:
        /// <ul>
        ///   <li><see cref="HttpContextBase.IsDebuggingEnabled"/> is <c>true</c></li>
        ///   <li><see cref="HttpContextBase.IsCustomErrorEnabled"/> is <c>false</c></li>
        ///   <li>the <strong>Accept</strong> header of the current request contains <c>text/html</c></li>
        /// </ul>
        /// </summary>
        public bool ShowHtmlErrors { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">A SPA request.</param>
        public SpaRequestOptions(SpaRequest request) : this(request.HttpContext) { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">A HTTP context.</param>
        public SpaRequestOptions(HttpContextBase context) {

            // Get a reference to the current request
            HttpRequestBase r = context.Request;
            
            // Get the host name from the query
            string appHost = r.QueryString["appHost"];

            // Get the protocol from the query
            string appProtocol = r.QueryString["appProtocol"];

            // Use the current URL as fallback for "appHost" and "appProtocol"
            HostName = string.IsNullOrWhiteSpace(appHost) ? r.Url?.Host : appHost;
            Protocol = string.IsNullOrWhiteSpace(appProtocol) ? r.Url?.Scheme : appProtocol;

            // Parse the "navLevels" and "navContext" parameters from the query string
            NavLevels = r.QueryString["navLevels"].ToInt32(1);
            NavContext = StringUtils.ParseBoolean(r.QueryString["navContext"]);

            // Parse the requests "parts"
            Parts = GetParts(r.QueryString["parts"]);

            // Get the URL of thr requested page
            Url = r.QueryString["url"];

            // Parse the "siteId" and "pageId" parameters ("appSiteId" and "nodeId" are checked for legacy support)
            SiteId = Math.Max(r.QueryString["siteId"].ToInt32(-1), r.QueryString["appSiteId"].ToInt32(-1));
            PageId = Math.Max(r.QueryString["pageId"].ToInt32(-1), r.QueryString["nodeId"].ToInt32(-1));

            QueryString = r.QueryString;

            // Determine whether the current request is in debug mode
            IsPreview = Url?.IsPreviewUrl() ?? false;

            // Determine whether caching should be enabled
            EnableCaching = context.IsDebuggingEnabled == false && StringUtils.ParseBoolean(r.QueryString["cache"], true) && IsPreview == false;

            ShowHtmlErrors = context.IsDebuggingEnabled && context.IsCustomErrorEnabled == false && context.Request.Headers["Accept"].Contains("text/html");

        }

        public SpaRequestOptions(int siteId) {
            SiteId = siteId;
            Url = "";
			IsPreview = false;
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url) {
            SiteId = siteId;
            Url = url;
			IsPreview = url.IsPreviewUrl();
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url, string parts) {
            SiteId = siteId;
			Url = string.IsNullOrWhiteSpace(url) ? "/" : url;
			IsPreview = Url.IsPreviewUrl();
			Parts = GetParts(parts);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts the specified string of <paramref name="parts"/> to <see cref="List{SpaApiPart}"/>.
        /// </summary>
        /// <param name="parts">The string with the parts.</param>
        /// <returns>An an instance of <see cref="List{SpaApiPart}"/> containing each <see cref="SpaApiPart"/> specified in <paramref name="parts"/>.</returns>
        private static List<SpaApiPart> GetParts(string parts = "") {
            
            // No parts means all parts
            if (String.IsNullOrWhiteSpace(parts)) return new List<SpaApiPart> { SpaApiPart.Content, SpaApiPart.Navigation, SpaApiPart.Site };

            List<SpaApiPart> temp = new List<SpaApiPart>();
            foreach (string item in parts.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                if (EnumUtils.TryParseEnum(item, out SpaApiPart part)) temp.Add(part);
            }

            return temp;

        }

        #endregion

    }

}