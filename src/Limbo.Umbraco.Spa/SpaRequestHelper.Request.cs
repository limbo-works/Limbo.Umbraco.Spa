using Limbo.Umbraco.Spa.Exceptions;
using Limbo.Umbraco.Spa.Json.Resolvers;
using Limbo.Umbraco.Spa.Models;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Limbo.Umbraco.Spa;

public partial class SpaRequestHelper {

    /// <summary>
    /// Virtual method for getting the <see cref="IPublishedContent"/> for the specified <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <returns>An instance of <see cref="IPublishedContent"/> representing the current page, or <c>null</c> if not found.</returns>
    protected virtual IPublishedContent GetContentFromRequest(SpaRequest request) {

        // Throw an exception if we don't have a site reference at this point
        if (request.Site == null) throw new PropertyNotSetException(nameof(request.Site));

        // Return NULL if we don't have an Umbraco context
        if (!UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext umbracoContext)) return null;

        // Get the page ID and/or URL parameters
        int pageId = request.Arguments.PageId;
        string url = request.Arguments.Url;
        if (pageId <= 0 && string.IsNullOrWhiteSpace(url)) throw new SpaException(request, "Both 'pageId' and 'url' parameters are missing.");

        // If the current domain specifies a path, we remove that from the path of the current request
        if (request.Domain is { Uri: { AbsolutePath.Length: > 1 } uri }) url = url[uri.AbsolutePath.Length..];

        // The domain may not be added to the site node, so we look for the content ID associated with the domain
        // first, and if not available, use the site ID as fallback
        int rootId = request.Domain?.ContentId ?? request.SiteId;

        // Attempt to get content item by either it's numeric ID or URL
        return pageId > 0 ? umbracoContext.Content?.GetById(pageId) : umbracoContext.Content?.GetByRoute($"{rootId}{url}", culture: request.CultureInfo.Name);

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

        // Return NULL if we dont have an Umbraco context
        if (!UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext umbracoContext)) return null;

        // Attemp to get the culture ID from the URL of the current request
        int contentId = GetCultureIdFromUrl(request);

        // Get the IPublishedContent matching "contentId"
        return contentId > 0 ? umbracoContext.Content?.GetById(contentId) : null;

    }

    /// <summary>
    /// Attempts to get the domain with the specified <paramref name="domainName"/>¨.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="domain">An instance of <see cref="IDomain"/> representing the domain, or <c>null</c> not found.</param>
    /// <returns><c>true</c> if a matching domain was found, otherwise <c>false</c>.</returns>
    protected virtual bool TryGetDomain(string domainName, out IDomain domain) {

        // TODO: Should we validate the domain name?

        domain = Services.DomainService!.GetByName(domainName);

        return domain != null;

    }

    /// <summary>
    /// Serializes the specified <paramref name="data"/> into a JSON string.
    /// </summary>
    /// <param name="data">The data to be serialized.</param>
    /// <returns>A JSON string.</returns>
    protected virtual string Serialize(object data) {

        return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings {
            ContractResolver = new SpaPublishedContentContractResolver()
        });

    }

}