using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaSiteModel {

        #region Properties

        /// <summary>
        /// Gets a reference to the root content node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Content { get; }

        /// <summary>
        /// Gets a reference to the content node that provides culture specific settings for the site.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Culture { get; }

        [JsonIgnore]
        public int CultureId => Culture?.Id ?? 0;

        /// <summary>
        /// Gets the ID of the site.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets the name of the site.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; protected set; }

        #endregion

        #region Constructors

        public SpaSiteModel(IPublishedContent site, IPublishedContent culture) {

            // Site
            Content = site;
            Id = Content.Id;

            // Culture
            Culture = culture;
            Name = culture.GetPropertyValue<string>("siteName");
            
        }

        #endregion

    }

}