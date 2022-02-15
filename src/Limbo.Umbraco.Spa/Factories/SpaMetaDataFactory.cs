using Skybrud.Essentials.Umbraco;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Meta;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Spa.Factories {
    
    /// <summary>
    /// The SPA's default implementation of the <see cref="ISpaMetaDataFactory"/> interface.
    /// </summary>
    public class SpaMetaDataFactory : ISpaMetaDataFactory {

        /// <inheritdoc/>
        public SpaMetaData GetMetaData(SpaContentModel content, SpaRequest request) {

            SpaMetaData meta = new SpaMetaData();

            meta.HtmlAttributes.Language = content.GetCultureInfo().ToString();

            meta.DangerouslyDisableSanitizers = new[] {"script"};

            meta.Canonical = content.Url(mode: UrlMode.Absolute);

            return meta;

        }

    }

}