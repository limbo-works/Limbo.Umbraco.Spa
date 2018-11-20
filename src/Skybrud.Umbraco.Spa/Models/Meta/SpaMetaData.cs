using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;

namespace Skybrud.Umbraco.Spa.Models.Meta {

    [JsonConverter(typeof(SpaMetaDataJsonConverter))]
    public class SpaMetaData {

        #region Properties

        /// <summary>
        /// Gets the meta title of the currrent page.
        /// </summary>
        [JsonProperty("title")]
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets the meta description of the currrent page.
        /// </summary>
        [JsonProperty("description")]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets the current page should be hidden in search results.
        /// </summary>
        [JsonIgnore]
        public bool HideFromSearch { get; set; }

        /// <summary>
        /// Gets the robots value for the current page.
        /// </summary>
        [JsonProperty("robots")]
        public string Robots { get; set; }

        /// <summary>
        /// Gets the Open Graph title for the current page.
        /// </summary>
        [JsonProperty("og:title")]
        public string OpenGraphTitle { get; set; }

        /// <summary>
        /// Gets the Open Graph description for the current page.
        /// </summary>
        [JsonProperty("og:description")]
        public string OpenGraphDescription { get; set; }

        /// <summary>
        /// Gets the Open Graph site name.
        /// </summary>
        [JsonProperty("og:site_name")]
        public string OpenGraphSiteName { get; set; }

        /// <summary>
        /// Gets the Open Graph URL for the current page.
        /// </summary>
        [JsonProperty("og:url")]
        public string OpenGraphUrl { get; set; }

        /// <summary>
        /// Gets a collection of Open Graph images for the current page.
        /// </summary>
        [JsonProperty("og:image")]
        public List<SpaOpenGraphImage> OpenGraphImages { get; set; }


        public List<SpaMetaScript> Scripts { get; set; }

        #endregion

        #region Constructors

        public SpaMetaData() {
            OpenGraphImages = new List<SpaOpenGraphImage>();
            Scripts = new List<SpaMetaScript>();
        }

        #endregion

    }

}