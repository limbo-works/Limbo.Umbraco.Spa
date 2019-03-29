using System;
using System.Globalization;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Umbraco.Spa.Extensions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaRequest {

        #region Properties

        public static SpaRequest Current {
            get {
                if (System.Web.HttpContext.Current == null) return null;
                return System.Web.HttpContext.Current.Items["SpaApiRequest"] as SpaRequest;
            }
            set => System.Web.HttpContext.Current.Items["SpaApiRequest"] = value;
        }

        /// <summary>
        /// Gets a reference to the HTTP context of the request.
        /// </summary>
        public HttpContextBase HttpContext { get; }

        /// <summary>
        /// Gets or sets the ID of the site.
        /// </summary>
        [JsonProperty("id")]
        public int SiteId { get; set; }

        /// <summary>
        /// Gets a reference to the <see cref="IPublishedContent"/> representing the site node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Site { get; set; }

        public SpaSiteModel SiteModel { get; set; }

        /// <summary>
        /// Gets a reference to the <see cref="IPublishedContent"/> representing the culture node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Culture { get; set; }

        /// <summary>
        /// Gets the URL of the current page.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets whether the user is currently in preview mode.
        /// </summary>
        [JsonProperty("isPreview")]
        public bool IsPreview { get; set; }

        /// <summary>
        /// Gets a collection of the parts being requested.
        /// </summary>
        [JsonProperty("parts")]
        public SpaApiPart[] Parts { get; set; }

        /// <summary>
        /// Gets the protocol of the current request.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets the host name of the current request.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the content item of the request.
        /// </summary>
        public IPublishedContent Content { get; set; }

        public object ContentModel { get; set; }

        /// <summary>
        /// Gets the virtual parent if present; otherwise <c>null</c>.
        /// 
        /// A virtual parent is typically used when a content item should appear under another node than it's own parent.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent VirtualParent { get; set; }

        /// <summary>
        /// Gets whether the request has a virtual parent.
        /// </summary>
        [JsonIgnore]
        public bool HasVirtualParent => VirtualParent != null;

        public bool IsDanish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da";

        public bool IsEnglish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new a instance with default options.
        /// </summary>
        [JsonConstructor]
        public SpaRequest() {
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        /// <summary>
        /// Intializes a new instance for the site with the specified <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        public SpaRequest(int siteId) {
            SiteId = siteId;
            Url = "";
            IsPreview = false;
            Parts = GetParts();
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        public SpaRequest(int siteId, string url) {
            SiteId = siteId;
            Url = url;
            IsPreview = url.IsPreviewUrl();
            Parts = GetParts();
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        public SpaRequest(int siteId, string url, string parts) {
            SiteId = siteId;
            Url = String.IsNullOrWhiteSpace(url) ? "/" : url;
            IsPreview = Url.IsPreviewUrl();
            Parts = GetParts(parts);
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        public SpaRequest(int siteId, IPublishedContent site, string url, string parts) {
            SiteId = siteId;
            Site = site;
            Url = String.IsNullOrWhiteSpace(url) ? "/" : url;
            IsPreview = Url.IsPreviewUrl();
            Parts = GetParts(parts);
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        public SpaRequest(UmbracoContext umbraco, int siteId, IPublishedContent site, string url, string parts) {
            SiteId = siteId;
            Site = site;
            Url = String.IsNullOrWhiteSpace(url) ? "/" : url;
            IsPreview = Url.IsPreviewUrl();
            Parts = GetParts(parts);
            HttpContext = umbraco.HttpContext;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts <paramref name="parts"/> to an array of <see cref="SpaApiPart"/>. If <paramref name="parts"/> is
        /// <c>null</c> or empty, an array will all parts will be returned.
        /// </summary>
        /// <param name="parts">The string with the parts to be parsed.</param>
        /// <returns>An array of <see cref="SpaApiPart"/>.</returns>
        private static SpaApiPart[] GetParts(string parts = "") {
            if (String.IsNullOrWhiteSpace(parts)) return new [] { SpaApiPart.Content, SpaApiPart.Navigation, SpaApiPart.Site };
            return EnumUtils.ParseEnumArray<SpaApiPart>(parts);
        }

        #endregion
    }

}