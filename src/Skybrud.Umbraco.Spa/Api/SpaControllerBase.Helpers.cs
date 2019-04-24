using System;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Json.Resolvers;
using Skybrud.Umbraco.Spa.Models;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa.Api {

    public abstract partial class SpaControllerBase {

        protected virtual IPublishedContent GetContentFromInput(IPublishedContent site, int nodeId, string url) {
            
            if (site == null) throw new ArgumentNullException(nameof(site));

            // Attempt to find the content item by it's route
            return UmbracoContext.ContentCache.GetByRoute(site.Id + url);

        }

        protected virtual int GetCultureIdFromUrl(SpaRequest request) {
            return -1;
        }

        protected virtual IPublishedContent GetCultureFromUrl(SpaRequest request) {
            int contentId = GetCultureIdFromUrl(request);
            return contentId > 0 ? UmbracoContext.ContentCache.GetById(contentId) : null;
        }

        protected virtual bool TryGetDomain(string domainName, out IDomain domain) {

	        // TODO: Should we validate the domain name?

	        domain = Current.Services.DomainService.GetByName(domainName);

	        return domain != null;

	    }

        protected virtual string Serialize(object data) {

            SpaGridJsonConverterBase gridConverter = new SpaGridJsonConverterBase();

            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings {
                ContractResolver = new SpaPublishedContentContractResolver(gridConverter)
            });

        }

    }

}