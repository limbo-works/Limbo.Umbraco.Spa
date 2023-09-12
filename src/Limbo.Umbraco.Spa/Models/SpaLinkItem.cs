using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing a simple link.
    /// </summary>
    public class SpaLinkItem {

        /// <summary>
        /// Gets the URL of the link.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the <c>target</c> attribute of the link.
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; }

        /// <summary>
        /// Initializes a new link based on the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the link.</param>
        protected SpaLinkItem(string url) {
            Url = url;
        }

        /// <summary>
        /// Initializes a new link based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The instance of <see cref="IPublishedContent"/> the link should be based on.</param>
        protected SpaLinkItem(IPublishedContent content) {
            Url = content.Url();
        }

        /// <summary>
        /// Initializes a new link based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The instance of <see cref="IPublishedContent"/> the link should be based on.</param>
        /// <returns>An instance of <see cref="SpaLinkItem"/>.</returns>
        public static SpaLinkItem GetFromContent(IPublishedContent content) {
            return content == null ? null : new SpaLinkItem(content);
        }

        /// <summary>
        /// Initializes a new link based on the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the link.</param>
        /// <returns>An instance of <see cref="SpaLinkItem"/>.</returns>
        public static SpaLinkItem GetFromUrl(string url) {
            return string.IsNullOrWhiteSpace(url) ? null : new SpaLinkItem(url);
        }

    }

}