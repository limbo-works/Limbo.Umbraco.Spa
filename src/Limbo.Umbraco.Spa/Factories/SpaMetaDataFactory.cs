using Limbo.MetaData.Models.OpenGraph;
using Limbo.MetaData.Models.Twitter;
using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Meta;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Factories {
    
    /// <summary>
    /// The SPA's default implementation of the <see cref="ISpaMetaDataFactory"/> interface.
    /// </summary>
    public class SpaMetaDataFactory : ISpaMetaDataFactory {

        /// <summary>
        /// Returns the meta data for the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The content item.</param>
        /// <param name="request">The current SPA request.</param>
        /// <returns>An instance of <see cref="SpaMetaData"/>.</returns>
        public virtual SpaMetaData CreateMetaData(IPublishedContent content, SpaRequest request) {

            // Initialize a new meta data instance
            var meta = new SpaMetaData(request.CultureInfo);

            // Update the various information with values from the virtual methods
            meta.Title = GetTitle(meta, content, request);
            meta.MetaTitle = GetMetaTitle(meta, content, request);
            meta.MetaDescription = GetMetaDescription(meta, content, request);
            meta.CanonicalUrl = GetCanonicalUrl(meta, content, request);
            meta.Robots = GetRobots(meta, content, request);
            meta.OpenGraph = GetOpenGraph(meta, content, request);
            meta.TwitterCard = GetTwitterCard(meta, content, request);

            // Return the meta data
            return meta;

        }

        /// <summary>
        /// Returns the canonical URL for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the canonical URL.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The canonical URL for <paramref name="content"/>.</returns>
        public virtual string GetCanonicalUrl(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {
            return content.Url(request.CultureInfo.ToString(), UrlMode.Absolute);
        }

        /// <summary>
        /// Returns the browser title (value for the <c>&lt;title&gt;</c> element) for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the browser title for.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The browser title.</returns>
        public virtual string GetTitle(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {
            return $"{GetMetaTitle(metaData, content, request)} - {request.Site.Name}";
        }

        /// <summary>
        /// Returns the page title for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the page title.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The page title.</returns>
        public virtual string GetMetaTitle(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {
            return content.Name;
        }

        /// <summary>
        /// Returns the meta description for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the page title.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The meta description.</returns>
        public virtual string GetMetaDescription(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {
            return content.Value<string>("teaser");
        }

        /// <summary>
        /// Returns the robots value for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the robots value.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The robots value.</returns>
        public virtual string GetRobots(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {

            if (request.Arguments.HostName.Contains("liveserver.nu")) {
                return "noindex, nofollow";
            }

            if (request.Arguments.HostName.Contains("testserver.nu")) {
                return "noindex, nofollow";
            }

            if (request.Arguments.HostName.Contains("azurewebsites.net")) {
                return "noindex, nofollow";
            }

            return content.Value<bool>("hideFromSearch") ? "noindex, follow" : "index, follow";

        }

        /// <summary>
        /// Returns the Open Graph information for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the Open Graph information.</param>
        /// <param name="request">The current request.</param>
        /// <returns>An instance of <see cref="OpenGraphProperties"/> representing the Open Graph information.</returns>
        public virtual OpenGraphProperties GetOpenGraph(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {

            OpenGraphProperties og = new() {
                Url = content.Url(mode: UrlMode.Absolute),
                SiteName = request.Site.Name,
                Title = content.Name,
                Description = metaData.MetaDescription
            };

            return og;

        }

        /// <summary>
        /// Returns the Twitter card for <paramref name="content"/>.
        /// </summary>
        /// <param name="metaData">The meta data instance.</param>
        /// <param name="content">The page for which to get the Twitter card.</param>
        /// <param name="request">The current request.</param>
        /// <returns>An instance of <see cref="ITwitterCard"/> representing the Twitter card.</returns>
        public virtual ITwitterCard GetTwitterCard(SpaMetaData metaData, IPublishedContent content, SpaRequest request) {

            return new TwitterSummaryCard {
                Site = request.Site.Name,
                Title = metaData.Title,
                Description = metaData.MetaDescription
            };

        }

    }

}