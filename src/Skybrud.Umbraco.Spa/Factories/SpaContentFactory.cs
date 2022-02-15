using Skybrud.Umbraco.Spa.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa.Factories {

    /// <summary>
    /// The SPA's default implementation of the <see cref="ISpaContentFactory"/> interface.
    /// </summary>
    public class SpaContentFactory : ISpaContentFactory {

        /// <inheritdoc/>
        public SpaContentModel CreateContentModel(IPublishedContent content, PublishedValueFallback publishedValueFallback, SpaRequest request) {

            switch (request.Content) {

                case PublishedContentModel pcm:
                    return new SpaContentModel(pcm, publishedValueFallback);

                default:
                    return null;

            }

        }

    }

}