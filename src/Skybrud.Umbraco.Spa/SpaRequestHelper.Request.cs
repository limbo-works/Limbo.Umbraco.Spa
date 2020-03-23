using System;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Json.Resolvers;
using Skybrud.Umbraco.Spa.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa {

    public partial class SpaRequestHelper {

        /// <summary>
        /// Virtual method for getting the <see cref="IPublishedContent"/> for the specified <paramref name="nodeId"/> or <paramref name="url"/>.
        /// </summary>
        /// <param name="site">The <see cref="IPublishedContent"/> representing the current site.</param>
        /// <param name="nodeId">The ID of the requested page.</param>
        /// <param name="url">The URL of the requested page.</param>
        /// <returns>An instance of <see cref="IPublishedContent"/> representing the current page, or <c>null</c> if not found.</returns>
        protected virtual IPublishedContent GetContentFromInput(IPublishedContent site, int nodeId, string url) {
            
            if (site == null) throw new ArgumentNullException(nameof(site));

            // Attempt to get content item by either it's numeric ID or URL
            return nodeId > 0 ? UmbracoContext.Content.GetById(nodeId) : UmbracoContext.Content.GetByRoute(site.Id + url);

        }

        /// <summary>
        /// Virtual method for getting the ID of the culture node of the current <paramref name="request"/>.
        ///
        /// The default implementation of this method will simply return <c>-1</c> as determining the culture node requires a custom implementation.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns>The numeric ID.</returns>
        protected virtual int GetCultureIdFromUrl(SpaRequest request) {
            return -1;
        }

        /// <summary>
        /// Virtual method for getting the <see cref="IPublishedContent"/> representing the culture node of the current <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns>An instance of <see cref="IPublishedContent"/> representing the culture node, or <c>null</c> if not found.</returns>
        protected virtual IPublishedContent GetCultureFromUrl(SpaRequest request) {
            int contentId = GetCultureIdFromUrl(request);
            return contentId > 0 ? UmbracoContext.Content.GetById(contentId) : null;
        }

        /// <summary>
        /// Attempts to get the domain with the specified <paramref name="domainName"/>¨.
        /// </summary>
        /// <param name="domainName">The domain name.</param>
        /// <param name="domain">An instance of <see cref="IDomain"/> representing the domain, or <c>null</c> not found.</param>
        /// <returns><c>true</c> if a matching domain was found, otherwise <c>false</c>.</returns>
        protected virtual bool TryGetDomain(string domainName, out IDomain domain) {

	        // TODO: Should we validate the domain name?

	        domain = Services.DomainService.GetByName(domainName);

	        return domain != null;

	    }

        /// <summary>
        /// Serializes the specified <paramref name="data"/> into a JSON string.
        /// </summary>
        /// <param name="data">The data to be serialized.</param>
        /// <returns>A JSON string.</returns>
        protected virtual string Serialize(object data) {

            SpaGridJsonConverterBase gridConverter = GridJsonConverter ?? new SpaGridJsonConverterBase();

            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings {
                ContractResolver = new SpaPublishedContentContractResolver(gridConverter)
            });

        }

    }

}