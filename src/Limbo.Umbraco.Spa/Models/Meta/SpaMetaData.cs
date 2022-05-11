using System.Globalization;
using Limbo.MetaData.Models;
using Limbo.Umbraco.Spa.Json.Converters;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Spa.Models.Meta {

    /// <summary>
    /// Class representing a meta data model.
    /// </summary>
    [JsonConverter(typeof(SpaMetaDataJsonConverter))]
    public class SpaMetaData : VueMetaData {

        /// <summary>
        /// Initializes a new instance with default options.
        /// </summary>
        public SpaMetaData() : base() { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="culture"/>.
        /// </summary>
        public SpaMetaData(CultureInfo culture) : base(culture) { }

    }

}