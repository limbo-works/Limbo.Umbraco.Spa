using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing a request via the <c>GetData</c> endpoint in the SPA API.
    /// </summary>
    public class SpaRequest {

        #region Properties

        /// <summary>
        /// Gets a reference to the HTTP context of the request.
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Gets the options/arguments determined from the current request.
        /// </summary>
        public SpaRequestOptions Arguments { get; set; }

        /// <summary>
        /// Gets or sets the ID of the site.
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// Gets a reference to the <see cref="IPublishedContent"/> representing the site node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Site { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="SpaSiteModel"/> representing the site model.
        /// </summary>
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
        public string Url => Arguments.Url;

        /// <summary>
        /// Gets whether the user is currently in preview mode.
        /// </summary>
        [JsonProperty("isPreview")]
        public bool IsPreview => Arguments.IsPreview;

        /// <summary>
        /// Gets a collection of the parts being requested.
        /// </summary>
        [JsonProperty("parts")]
        public SpaApiPart[] Parts => Arguments.Parts.ToArray();

        /// <summary>
        /// Gets the protocol of the current request.
        /// </summary>
        public string Protocol => Arguments.Protocol;

        /// <summary>
        /// Gets the host name of the current request.
        /// </summary>
        public string HostName => Arguments.HostName;

        /// <summary>
        /// Gets or sets the content item of the request.
        /// </summary>
        public IPublishedContent Content { get; set; }

        /// <summary>
        /// Gets or sets the content model.
        /// </summary>
        public SpaContentModel ContentModel { get; set; }

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

        /// <summary>
        /// Gets whether the request uses a Danish culture.
        /// </summary>
        public bool IsDanish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da";

        /// <summary>
        /// Gets whether the request uses an English culture.
        /// </summary>
        public bool IsEnglish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en";

        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        public SpaDataModel DataModel { get; set; }

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public ActionResult Response { get; set; }

        /// <summary>
        /// Gets the <see cref="Stopwatch"/> used for measuring the duration of the request.
        /// </summary>
        public Stopwatch Stopwatch { get; }

        /// <summary>
        /// Gets a reference to the domain of the request.
        /// </summary>
        public DomainAndUri Domain { get; set; }

        /// <summary>
        /// Gets a reference to the culture of the request.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new a instance based on the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        public SpaRequest(HttpContext context) {
            HttpContext = context ?? throw new ArgumentNullException(nameof(context));
            Stopwatch = Stopwatch.StartNew();
        }

        #endregion

    }

}