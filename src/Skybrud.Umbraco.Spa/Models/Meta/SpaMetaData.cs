using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Spa.Extensions;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Models.Meta.Attributes;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;
using Skybrud.Umbraco.Spa.Models.Meta.Twitter;
using Umbraco.Core;
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

        private SpaOpenGraphProperties _og;
        private ITwitterCard _twitter;

        #region Properties

        /// <summary>
        /// Gets a reference to the current page.
        /// </summary>
        protected IPublishedContent Content { get; }

        /// <summary>
        /// Gets or sets a list of attributes of the <c>html</c> element.
        /// </summary>
        public SpaMetaHtmlAttributeList HtmlAttributes { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes of the <c>head</c> element.
        /// </summary>
        public SpaMetaAttributeList HeadAttributes { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes of the <c>body</c> element.
        /// </summary>
        public SpaMetaAttributeList BodyAttributes { get; set; }

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
        public SpaOpenGraphProperties OpenGraph => _og ?? (_og = GetOpenGraph());

        /// <summary>
        /// Gets an instance of <see cref="ITwitterCard"/> representing the current page, or <c>null</c> if the page doesn't have a Twitter card..
        /// </summary>
        public ITwitterCard TwitterCard => _twitter ?? (_twitter = GetTwitterCard());

        /// <summary>
        /// Gets whether the page has a Twitter card.
        /// </summary>
        public bool HasTwitterCard => TwitterCard != null;

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
        /// <see>
        ///     <cref>https://github.com/nuxt/vue-meta/tree/1.x#__dangerouslydisablesanitizers-string</cref>
        /// </see>
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

            HtmlAttributes = new SpaMetaHtmlAttributeList {
                Language = content.GetCultureInfo().ToString()
            };

            HeadAttributes = new SpaMetaAttributeList();
            BodyAttributes = new SpaMetaAttributeList();

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
        /// Gets the Twitter card of the current page. This method will return <c>null</c> by default, but can be overriden by subclasses.
        /// </summary>
        /// <returns>An instance of <see cref="ITwitterCard"/>, or <c>null</c> it not available.</returns>
        protected virtual ITwitterCard GetTwitterCard() {
            return null;
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

        /// <summary>
        /// Writes the meta data to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteJson(JsonWriter writer) {
            ToMetaJson()?.WriteTo(writer);
        }

        /// <summary>
        /// Returns a <see cref="JObject"/> representing the meta data.
        /// </summary>
        public virtual JObject ToMetaJson() {

            JObject obj = new JObject();
            JArray meta = new JArray();

            obj["title"] = MetaTitle ?? string.Empty;

            // Append the <html> attributes (if any)
            if (HtmlAttributes != null && HtmlAttributes.Count > 0) {
                JObject htmlAttrs = new JObject();
                foreach (var (key, value) in HtmlAttributes) {
                    htmlAttrs.Add(key, value);
                }
                obj["htmlAttrs"] = htmlAttrs;
            }

            // Append the <head> attributes (if any)
            if (HeadAttributes != null && HeadAttributes.Count > 0) {
                JObject headAttrs = new JObject();
                foreach (var (key, value) in HeadAttributes) {
                    headAttrs.Add(key, value);
                }
                obj["headAttrs"] = headAttrs;
            }

            // Append the <body> attributes (if any)
            if (BodyAttributes != null && BodyAttributes.Count > 0) {
                JObject bodyAttrs = new JObject();
                foreach (var (key, value) in BodyAttributes) {
                    bodyAttrs.Add(key, value);
                }
                obj["bodyAttrs"] = bodyAttrs;
            }


            obj["meta"] = meta;

            SpaUtils.Json.AddMetaContent(meta, "description", MetaDescription ?? string.Empty, true);
            SpaUtils.Json.AddMetaContent(meta, "robots", Robots);

            OpenGraph?.WriteJson(meta);
            TwitterCard?.WriteJson(meta);

            if (Links.Count > 0) obj.Add("link", JArray.FromObject(Links.Where(x => x.IsValid)));
            if (Scripts.Count > 0) obj.Add("script", JArray.FromObject(Scripts));

            if (DangerouslyDisableSanitizers.Length > 0) obj.Add("__dangerouslyDisableSanitizers", new JArray(from str in DangerouslyDisableSanitizers select str));

            return obj;

        }

        #endregion

    }

}