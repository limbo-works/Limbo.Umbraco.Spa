using Limbo.Umbraco.Spa.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Spa.Factories {

    /// <summary>
    /// Interface describing a factory for creating SPA content models.
    /// </summary>
    public interface ISpaContentFactory {

        /// <summary>
        /// Returns a new SPA content model based on the specified <paramref name="content"/>, <paramref name="publishedValueFallback"/> and <paramref name="request"/>.
        /// </summary>
        /// <param name="content">The content item.</param>
        /// <param name="publishedValueFallback">The fallback values.</param>
        /// <param name="request">The current SPA request.</param>
        /// <returns>An instance of <see cref="SpaContentModel"/>.</returns>
        SpaContentModel CreateContentModel(IPublishedContent content, PublishedValueFallback publishedValueFallback, SpaRequest request);

        /// <summary>
        /// Attempts to create a new SPA content model based on the specified <paramref name="content"/>, <paramref name="publishedValueFallback"/> and <paramref name="request"/>.
        /// </summary>
        /// <param name="content">The content item.</param>
        /// <param name="publishedValueFallback">The fallback values.</param>
        /// <param name="request">The current SPA request.</param>
        /// <param name="contentModel">When this method returls, holds an instance of <see cref="SpaContentModel"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        bool TryCreateContentModel(IPublishedContent content, PublishedValueFallback publishedValueFallback, SpaRequest request, out SpaContentModel contentModel) {
            contentModel = CreateContentModel(content, publishedValueFallback, request);
            return contentModel != null;
        }

    }

}