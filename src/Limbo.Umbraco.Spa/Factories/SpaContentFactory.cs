using Limbo.Umbraco.Spa.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Spa.Factories {

    /// <summary>
    /// The SPA's default implementation of the <see cref="ISpaContentFactory"/> interface.
    /// </summary>
    public class SpaContentFactory : ISpaContentFactory {

        /// <inheritdoc/>
        public SpaContentModel CreateContentModel(IPublishedContent content, PublishedValueFallback publishedValueFallback, SpaRequest request) {
            return request.Content switch {
                PublishedContentModel pcm => new SpaContentModel(pcm, publishedValueFallback),
                _ => null
            };
        }

    }

}