using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Umbraco.Spa.Extensions;

namespace Skybrud.Umbraco.Spa.Api.Models {

    public class SpaApiRequest {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the site.
        /// </summary>
        [JsonProperty("id")]
        public int SiteId { get; set; }

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new a instance with default options.
        /// </summary>
        [JsonConstructor]
        public SpaApiRequest() { }

        /// <summary>
        /// Intializes a new instance for the site with the specified <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        public SpaApiRequest(int siteId) {
            SiteId = siteId;
            Url = "";
            IsPreview = false;
            Parts = GetParts();
        }

        public SpaApiRequest(int siteId, string url) {
            SiteId = siteId;
            Url = url;
            IsPreview = url.IsPreviewUrl();
            Parts = GetParts();
        }

        public SpaApiRequest(int siteId, string url, string parts) {
            SiteId = siteId;
            Url = String.IsNullOrWhiteSpace(url) ? "/" : url;
            IsPreview = Url.IsPreviewUrl();
            Parts = GetParts(parts);
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