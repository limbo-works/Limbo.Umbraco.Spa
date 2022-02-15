using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing a basic content model in context of the SPA.
    /// </summary>
    public class SpaContentModel : PublishedContentModel {

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> representing the current page.</param>
        public SpaContentModel(IPublishedContent content) : base(content, null) { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> representing the current page.</param>
        /// <param name="publishedValueFallback">The published value fallback.</param>
        public SpaContentModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback) { }

    }

}