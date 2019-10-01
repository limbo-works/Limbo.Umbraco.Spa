﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;
using Umbraco.Core.Models.PublishedContent;

// ReSharper disable UnusedParameter.Local

namespace Skybrud.Umbraco.Spa.Models.Meta {

    [JsonConverter(typeof(SpaMetaDataJsonConverter))]
    public class SpaMetaData {

        private readonly SpaMetaLink _canonical;

        #region Properties

        protected IPublishedContent Content { get; }

        /// <summary>
        /// Gets or sets the canonical URL of the current page.
        /// </summary>
        public string Canonical {
            get => _canonical.Href;
            set => _canonical.Href = value;
        }

        /// <summary>
        /// Gets the meta title of the current page.
        /// </summary>
        [JsonProperty("title")]
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets the meta description of the current page.
        /// </summary>
        [JsonProperty("description")]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets the current page should be hidden in search results.
        /// </summary>
        [JsonIgnore]
        public bool HideFromSearch { get; set; }

        /// <summary>
        /// Gets the robots value for the current page.
        /// </summary>
        [JsonProperty("robots")]
        public string Robots { get; set; }

        /// <summary>
        /// Gets a collection of Open Graph properties for the current page.
        /// </summary>
        public SpaOpenGraphProperties OpenGraph => GetOpenGraph();

        public List<SpaMetaLink> Links { get; set; }

        public List<SpaMetaScript> Scripts { get; set; }

        public string[] DangerouslyDisableSanitizers { get; set; }

        #endregion

        #region Constructors

        public SpaMetaData(SpaSiteModel site, IPublishedContent content) {

            Content = content;

            Links = new List<SpaMetaLink>();
            Scripts = new List<SpaMetaScript>();

            AddLink(_canonical = new SpaMetaLink { Rel = "canonical" });

            DangerouslyDisableSanitizers = new[] {"script"};

        }

        #endregion

        #region Member methods

        protected virtual SpaOpenGraphProperties GetOpenGraph() {
            return new SpaOpenGraphProperties(Content);
        }

        public SpaMetaLink AddLink(string href, string rel = null, string type = null, string media = null, string sizes = null) {
            return AddLink(new SpaMetaLink {
                Href = href,
                Rel = rel,
                Type = type,
                Media = media,
                Sizes = sizes
            });
        }

        public SpaMetaLink AddLink(SpaMetaLink link) {
            Links.Add(link);
            return link;
        }
        
        public SpaMetaScript AddScript(string source, string type = null) {
            return AddScript(new SpaMetaScript {
                Source = source,
                Type = type
            });
        }

        public SpaMetaScript AddScript(SpaMetaScript script) {
            Scripts.Add(script);
            return script;
        }

        #endregion

    }

}