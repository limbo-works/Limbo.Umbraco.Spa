using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing a basic content model in context of the SPA.
    /// </summary>
    public class SpaContentModel : PublishedContentModel {

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> representing the current page.</param>
        public SpaContentModel(IPublishedContent content) : base(content) { }

    }

}