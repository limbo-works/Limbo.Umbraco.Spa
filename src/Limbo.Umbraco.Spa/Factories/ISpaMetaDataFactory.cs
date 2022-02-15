using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Meta;

namespace Limbo.Umbraco.Spa.Factories {

    /// <summary>
    /// Interface describing a meta data factory.
    /// </summary>
    public interface ISpaMetaDataFactory {

        /// <summary>
        /// Returns the meta data for the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The content item.</param>
        /// <param name="request">The current SPA request.</param>
        /// <returns>An instance of <see cref="SpaMetaData"/>.</returns>
        public SpaMetaData GetMetaData(SpaContentModel content, SpaRequest request);

    }

}