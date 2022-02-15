using System;
using Limbo.Umbraco.Spa.Models.Navigation;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Models {

    /// <summary>
    /// Class representing the base model of a SPA site.
    /// </summary>
    public class SpaSiteModel {

        #region Properties

        /// <summary>
        /// Gets a reference to the root content node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Site { get; }

        /// <summary>
        /// Gets a reference to the content node that provides culture specific settings for the site.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Culture { get; }

        /// <summary>
        /// Gets the ID of the culture node, or <c>0</c> if the site context doesn't have a culture node.
        /// </summary>
        [JsonIgnore]
        public int CultureId => Culture?.Id ?? 0;

        /// <summary>
        /// Gets whether the site context has a reference to a culture node.
        /// </summary>
        [JsonIgnore]
        public bool HasCulture => Culture != null;

        /// <summary>
        /// Gets the ID of the site.
        /// </summary>
        [JsonProperty("id", Order = -100)]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets the name of the site.
        /// </summary>
        [JsonProperty("name", Order = -95)]
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the URL of the site or culture node.
        /// </summary>
        [JsonProperty("url", Order = -90)]
        public string Url { get; protected set; }

        /// <summary>
        /// Gets a reference to the <see cref="IPublishedContent"/> representing the 404 page of the requested site (or culture).
        /// </summary>
        [JsonIgnore]
        public IPublishedContent NotFoundPage { get; protected set; }

        /// <summary>
        /// Gets whether the site has a 404 page.
        /// </summary>
        [JsonIgnore]
        public bool HasNotFoundPage => NotFoundPage != null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new site model from the specified <paramref name="site"/>.
        /// 
        /// When this constructor is used, it's assumed that the site doesn't use cultures.
        /// </summary>
        /// <param name="site">An instance of <see cref="IPublishedContent"/> representing the site node.</param>
        public SpaSiteModel(IPublishedContent site) {
            
            // Site
            Site = site ?? throw new ArgumentNullException(nameof(site));
            Id = site.Id;
            Name = site.Value<string>(SpaConstants.Properties.SiteName);
            Url = site.Url();

        }

        /// <summary>
        /// Initializes a new site model from the specified <paramref name="site"/> and <paramref name="culture"/>.
        /// 
        /// When this constructor is used, a culture node must be present.
        /// </summary>
        /// <param name="site">An instance of <see cref="IPublishedContent"/> representing the site node.</param>
        /// <param name="culture">An instance of <see cref="IPublishedContent"/> representing the culture node.</param>
        public SpaSiteModel(IPublishedContent site, IPublishedContent culture) {
            
            // Site
            Site = site ?? throw new ArgumentNullException(nameof(site));
            Id = site.Id;

            // Culture
            Culture = culture ?? site;
            Name = site.Value<string>(SpaConstants.Properties.SiteName);
            Url = site.Url();

        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets the navigation model of this site.
        /// </summary>
        /// <returns>An instance of <see cref="SpaNavigationModel"/>.</returns>
        public virtual SpaNavigationModel GetNavigation() {
            return new SpaNavigationModel();
        }

        #endregion

    }

}