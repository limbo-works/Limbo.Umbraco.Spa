using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Meta {

    public class SpaMetaScript {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the script element.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the <c>title</c> attribtue of the script element.
        /// 
        /// A title attribute is typically not used for <c>scrippt</c> elements, but some integrations use it to pass
        /// on the title to a UI component.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the source (<c>src</c> attribute) of the script element.
        /// </summary>
        [JsonProperty("src", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the <c>type</c> attribute of the script element.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the inner HTML of the script element.
        /// </summary>
        [JsonProperty("innerHTML", NullValueHandling = NullValueHandling.Ignore)]
        public string InnerHtml { get; set; }

        /// <summary>
        /// Gets or sets whether the script element should be appended to the <c>&lt;body&gt;</c> element.
        /// </summary>
        [JsonProperty("body", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AppendToBody { get; set; }

        [JsonProperty("defer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Defer { get; set; }

        [JsonProperty("async", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Async { get; set; }

        #endregion

        #region Constructors

        public SpaMetaScript() {
            Type = "text/javascript";
        }

        #endregion

    }

}