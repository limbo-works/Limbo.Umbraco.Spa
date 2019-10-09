using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;
using Umbraco.Core.Models.PublishedContent;

// ReSharper disable UnusedParameter.Local

namespace Skybrud.Umbraco.Spa.Models.Meta {
    
    /// <summary>
    /// Class representing meta data about a page in Umbraco.
    /// 
    /// When serialized to JSON, the generated value uses a format which can be used in <strong>vue-meta</strong>.
    /// </summary>
    /// <see>
    ///     <cref>https://github.com/nuxt/vue-meta/tree/1.x</cref>
    /// </see>
    [JsonConverter(typeof(SpaMetaDataJsonConverter))]
    public class SpaMetaData {

        private readonly SpaMetaLink _canonical;

        #region Properties

        /// <summary>
        /// Gets a reference to the current page.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a collection of <c>&lt;link&gt;</c> elements.
        /// </summary>
        public List<SpaMetaLink> Links { get; set; }

        /// <summary>
        /// Gets or sets a collection of <c>&lt;script&gt;</c> elements.
        /// </summary>
        public List<SpaMetaScript> Scripts { get; set; }

        /// <summary>
        /// Gets or sets an array of properties where sanitizing should be disabled. 
        /// </summary>
        public string[] DangerouslyDisableSanitizers { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="site"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="site">The site of the current page.</param>
        /// <param name="content">The current page.</param>
        public SpaMetaData(SpaSiteModel site, IPublishedContent content) {

            Content = content;

            Links = new List<SpaMetaLink>();
            Scripts = new List<SpaMetaScript>();

            AddLink(_canonical = new SpaMetaLink { Rel = "canonical" });

            DangerouslyDisableSanitizers = new[] {"script"};

        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets the Open Graph properties for <see cref="Content"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual SpaOpenGraphProperties GetOpenGraph() {
            return new SpaOpenGraphProperties(Content);
        }

        /// <summary>
        /// Adds a new <c>&lt;link%gt;</c> element with the specified parameters.
        /// </summary>
        /// <param name="href">The value of the <c>href</c> attribute.</param>
        /// <param name="rel">The value of the <c>rel</c> attribute.</param>
        /// <param name="type">The value of the <c>type</c> attribute.</param>
        /// <param name="media">The value of the <c>media</c> attribute.</param>
        /// <param name="sizes">The value of the <c>sizes</c> attribute.</param>
        /// <returns>An instance of <see cref="SpaMetaLink"/> representing the added <c>&lt;link%gt;</c> element.</returns>
        public SpaMetaLink AddLink(string href, string rel = null, string type = null, string media = null, string sizes = null) {
            return AddLink(new SpaMetaLink {
                Href = href,
                Rel = rel,
                Type = type,
                Media = media,
                Sizes = sizes
            });
        }

        /// <summary>
        /// Adds a new <c>&lt;link%gt;</c> element based on <paramref name="link"/>.
        /// </summary>
        /// <param name="link">The <c>&lt;link%gt;</c> element to be added.</param>
        /// <returns>An instance of <see cref="SpaMetaLink"/> representing the added <c>&lt;link%gt;</c> element.</returns>
        public SpaMetaLink AddLink(SpaMetaLink link) {
            Links.Add(link);
            return link;
        }

        /// <summary>
        /// Adds a new <c>&lt;script%gt;</c> element with the specified <paramref name="source"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="source">The value of the <c>src</c> attribute.</param>
        /// <param name="type">The value of the <c>type</c> attribute.</param>
        /// <returns>An instance of <see cref="SpaMetaScript"/> representing the added <c>&lt;script%gt;</c> element.</returns>
        public SpaMetaScript AddScript(string source, string type = null) {
            return AddScript(new SpaMetaScript {
                Source = source,
                Type = type
            });
        }

        /// <summary>
        /// Adds a new <c>&lt;script%gt;</c> element based on <paramref name="script"/>.
        /// </summary>
        /// <param name="script">The <c>&lt;script%gt;</c> element to be added.</param>
        /// <returns>An instance of <see cref="SpaMetaScript"/> representing the added <c>&lt;script%gt;</c> element.</returns>
        public SpaMetaScript AddScript(SpaMetaScript script) {
            Scripts.Add(script);
            return script;
        }

        #endregion

    }

}