using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Meta {
    
    /// <summary>
    /// Represents a <c>link</c> HTML element.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.mozilla.org/en-US/docs/Web/HTML/Element/link</cref>
    /// </see>
    public class SpaMetaLink {

        #region Properties

        /// <summary>
        /// Gets or sets the relationship of the linked document to the current document. The value must be a
        /// space-separated list of the
        /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Link_types">link types values</a>.
        /// </summary>
        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Rel { get; set; }

        /// <summary>
        /// This attribute is used to define the type of the content linked to. The value of the attribute should be a
        /// MIME type such as <c>text/html</c>, <c>text/css</c>, and so on. The common use of this attribute is to
        /// define the type of stylesheet being referenced (such as <c>text/css</c>), but given that CSS is the only
        /// stylesheet language used on the web, not only is it possible to omit the type attribute, but is actually
        /// now recommended practice. It is also used on <c>rel="preload"</c> link types, to make sure the browser only
        /// downloads file types that it supports.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// This attribute specifies the URL of the linked resource. A URL can be absolute or relative.
        /// </summary>
        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        /// <summary>
        /// This attribute specifies the media that the linked resource applies to. Its value must be a media type / media query.
        /// This attribute is mainly useful when linking to external stylesheets — it allows the user agent to pick the best adapted
        /// one for the device it runs on.
        /// </summary>
        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
        public string Media { get; set; }

        /// <summary>
        /// This attribute defines the sizes of the icons for visual media contained in the resource. It must be
        /// present only if the rel contains a value of icon or a non-standard type such as Apple's
        /// <c>apple-touch-icon</c>. It may have the following values:
        /// 
        /// <ul>
        ///     <li>
        ///         any, meaning that the icon can be scaled to any size as it is in a vector format, like <c>image/svg+xml</c>.
        ///     </li>
        ///     <li>
        ///         a white-space separated list of sizes, each in the format <c>&lt;width in pixels&gt;x&lt;height in pixels&gt;</c>
        ///         or <c>&lt;width in pixels&gt;X&lt;height in pixels&gt;</c>. Each of these sizes must be contained in the resource.
        ///     </li>
        /// </ul>
        /// </summary>
        [JsonProperty("sizes", NullValueHandling = NullValueHandling.Ignore)]
        public string Sizes { get; set; }

        [JsonIgnore]
        public virtual bool IsValid => String.IsNullOrWhiteSpace(Href) == false;

        #endregion

        #region Constructors

        public SpaMetaLink() { }

        public SpaMetaLink(string href, string rel = null, string type = null) {
            Href = href;
            Rel = rel;
            Type = type;
        }

        #endregion

    }

}