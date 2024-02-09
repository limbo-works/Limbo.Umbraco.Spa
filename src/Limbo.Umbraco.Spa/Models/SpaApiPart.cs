using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Enums class representing the parts of a SPA response.
    /// </summary>
    [JsonConverter(typeof(EnumStringConverter))]
    public enum SpaApiPart {

        /// <summary>
        /// Indicates that the SPA API should return the <strong>Content</strong> part.
        /// </summary>
        Content,

        /// <summary>
        /// Indicates that the SPA API should return the <strong>Navigation</strong> part.
        /// </summary>
        Navigation,

        /// <summary>
        /// Indicates that the SPA API should return the <strong>Site</strong> part.
        /// </summary>
        Site

    }

}