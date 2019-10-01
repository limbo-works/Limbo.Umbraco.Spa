﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaRequest {

        #region Properties

        /// <summary>
        /// Gets a reference to the current SPA API request.
        /// </summary>
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
        /// Gets the options/arguments determined from the current request.
        /// </summary>
        public SpaRequestOptions Arguments { get; }

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

        public bool IsDanish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da";

        public bool IsEnglish => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en";

        public SpaDataModel DataModel { get; set; }

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

        public HttpResponseMessage Response { get; set; }

        public Stopwatch Stopwatch { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new a instance with default options.
        /// </summary>
        [JsonConstructor]
        public SpaRequest() {
            HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
            Arguments = new SpaRequestOptions(this);
            Stopwatch = Stopwatch.StartNew();
        }

        public SpaRequest(HttpContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            HttpContext = new HttpContextWrapper(context);
            Arguments = new SpaRequestOptions(this);
            Stopwatch = Stopwatch.StartNew();
        }
        
        #endregion

    }

}