using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Limbo.Umbraco.Spa.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Repositories;

/// <summary>
/// Repository for working with Umbraco domains in relation to the SPA package.
/// </summary>
public class SpaDomainRepository {

    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
    private readonly ISiteDomainMapper _siteDomainMapper;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="publishedSnapshotAccessor">The current published snapshot accessor.</param>
    /// <param name="siteDomainMapper">The current site domain mapper.</param>
    public SpaDomainRepository(IPublishedSnapshotAccessor publishedSnapshotAccessor, ISiteDomainMapper siteDomainMapper) {
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
        _siteDomainMapper = siteDomainMapper;
    }

    /// <summary>
    /// Returns the <see cref="DomainAndUri"/> for the node with the specified <paramref name="nodeId"/>, or
    /// <c>null</c> if not found. The domain will be determined either from the node it self or one of it's ancestors.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <param name="current">The URI of the request.</param>
    /// <param name="culture">The culture code of the request.</param>
    /// <returns>An instance of <see cref="DomainAndUri"/> representing the domain, or <c>null</c> if not domain was found.</returns>
    public DomainAndUri DomainForNode(int nodeId, Uri current, string culture = null) {

        // Attempt to get a reference to the current snapshot
        if (!_publishedSnapshotAccessor.TryGetPublishedSnapshot(out IPublishedSnapshot snapshot)) return null;

        // be safe
        if (nodeId <= 0) return null;

        // get the domains on that node
        var domains = snapshot!.Domains!.GetAssigned(nodeId).ToArray();

        // none?
        if (domains.Length == 0) return null;

        // else filter
        // it could be that none apply (due to culture)
        return SelectDomain(domains, current, culture, snapshot.Domains.DefaultCulture, _siteDomainMapper.MapDomain);

    }

    /// <summary>
    /// Returns the <see cref="DomainAndUri"/> for the specified <paramref name="content"/> node, or <c>null</c> if
    /// not found. The domain will be determined either from the node it self or one of it's ancestors.
    /// </summary>
    /// <param name="content">Ther node.</param>
    /// <param name="current">The URI of the request.</param>
    /// <param name="culture">The culture code of the request.</param>
    /// <returns>An instance of <see cref="DomainAndUri"/> representing the domain, or <c>null</c> if not domain was found.</returns>
    public DomainAndUri DomainForNode(IPublishedContent content, Uri current, string culture = null) {

        while (content != null) {

            DomainAndUri domain = DomainForNode(content.Id, current, culture);
            if (domain != null) return domain;

            content = content.Parent;

        }

        return null;

    }

    /// <summary>
    /// Attempts to find the domain for the specified <paramref name="request"/> and <paramref name="uri"/>. If a
    /// domain is found, it will be used to populate the <see cref="SpaRequest.Domain"/> property of <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="uri">The URI.</param>
    /// <returns><c>true</c> if a domain was found; otherwise <c>false</c>.</returns>
    public bool FindDomain(SpaRequest request, Uri uri) {

        // Attempt to get a reference to the current snapshot
        if (!_publishedSnapshotAccessor.TryGetPublishedSnapshot(out IPublishedSnapshot snapshot)) return false;

        // If a page ID was specifically specified for the request, it may mean that we're
        // in preview mode or that the "url" parameter isn't specified. In either case, we
        // need to find the assigned domains of the requested node (or it's ancestor) so we
        // can determine the sitenode

        if (request.Arguments.PageId > 0) {

            // If a page ID was specifically specified for the request, it may mean that we're
            // in preview mode or that the "url" parameter isn't specified. In either case, we
            // need to find the assigned domains of the requested node (or it's ancestor) so we
            // can determine the site node

            // TODO: Look at the "siteId" parameter as well (may be relevant for virtual content etc.)

            IPublishedContent c = snapshot!.Content?.GetById(request.Arguments.PageId);

            if (c != null) {
                request.Domain = DomainForNode(c, null, request.Arguments.Culture);
                if (!string.IsNullOrWhiteSpace(request.Domain?.Culture)) {
                    request.CultureInfo = CultureInfo.GetCultureInfo(request.Domain.Culture);
                    return true;
                }
            }

            // TODO: Should we return "false" here if no domains are found?

        }

        var domainsCache = snapshot!.Domains!;
        var domains = domainsCache.GetAll(includeWildcards: false).ToList();

        // determines whether a domain corresponds to a published document, since some
        // domains may exist but on a document that has been unpublished - as a whole - or
        // that is not published for the domain's culture - in which case the domain does
        // not apply
        bool IsPublishedContentDomain(Domain domain) {

            // just get it from content cache - optimize there, not here
            var domainDocument = snapshot.Content?.GetById(domain.ContentId);

            // not published - at all
            if (domainDocument == null)
                return false;

            // invariant - always published
            if (!domainDocument.ContentType.VariesByCulture())
                return true;

            // variant, ensure that the culture corresponding to the domain's language is published
            return domainDocument.Cultures.ContainsKey(domain.Culture);

        }

        domains = domains.Where(IsPublishedContentDomain).ToList();

        var defaultCulture = domainsCache.DefaultCulture;

        // try to find a domain matching the current request
        var domainAndUri = SelectDomain(domains, uri, defaultCulture: defaultCulture);

        // handle domain - always has a contentId and a culture
        if (!string.IsNullOrWhiteSpace(domainAndUri?.Culture)) {
            request.Domain = domainAndUri;
            request.CultureInfo = CultureInfo.GetCultureInfo(domainAndUri.Culture);

        } else {
            request.CultureInfo = new CultureInfo(defaultCulture);
        }

        return request.Domain != null;

    }

    private DomainAndUri SelectDomain(IEnumerable<Domain> domains, Uri uri, string culture = null, string defaultCulture = null, Func<IReadOnlyCollection<DomainAndUri>, Uri, string, string, DomainAndUri> filter = null) {

        // sanitize the list to have proper uris for comparison (scheme, path end with /)
        // we need to end with / because example.com/foo cannot match example.com/foobar
        // we need to order so example.com/foo matches before example.com/
        var domainsAndUris = domains
            .Where(d => d.IsWildcard == false)
            .Select(d => new DomainAndUri(d, uri))
            .OrderByDescending(d => d.Uri.ToString())
            .ToList();

        // nothing = no magic, return null
        if (domainsAndUris.Count == 0)
            return null;

        // sanitize cultures
        culture = culture?.NullOrWhiteSpaceAsNull();
        defaultCulture = defaultCulture?.NullOrWhiteSpaceAsNull();

        if (uri == null) {
            // no uri - will only rely on culture
            return GetByCulture(domainsAndUris, culture, defaultCulture);
        }

        // else we have a uri,
        // try to match that uri, else filter

        // if a culture is specified, then try to get domains for that culture
        // (else cultureDomains will be null)
        // do NOT specify a default culture, else it would pick those domains
        var cultureDomains = SelectByCulture(domainsAndUris, culture, defaultCulture: null);

        IReadOnlyCollection<DomainAndUri> considerForBaseDomains = domainsAndUris;
        if (cultureDomains != null) {
            if (cultureDomains.Count == 1) // only 1, return
                return cultureDomains.First();

            // else restrict to those domains, for base lookup
            considerForBaseDomains = cultureDomains;
        }

        // look for domains that would be the base of the uri
        var baseDomains = SelectByBase(considerForBaseDomains, uri);
        if (baseDomains.Count > 0) // found, return
            return baseDomains.First();

        // if nothing works, then try to run the filter to select a domain
        // either restricting on cultureDomains, or on all domains
        if (filter != null) {
            var domainAndUri = filter(cultureDomains ?? domainsAndUris, uri, culture, defaultCulture);
            // if still nothing, pick the first one?
            // no: move that constraint to the filter, but check
            if (domainAndUri == null)
                throw new InvalidOperationException("The filter returned null.");
            return domainAndUri;
        }

        return null;

    }

    private bool IsBaseOf(DomainAndUri domain, Uri uri) => domain.Uri.EndPathWithSlash().IsBaseOf(uri);

    private IReadOnlyCollection<DomainAndUri> SelectByBase(IReadOnlyCollection<DomainAndUri> domainsAndUris, Uri uri) {
        // look for domains that would be the base of the uri
        // ie current is www.example.com/foo/bar, look for domain www.example.com
        var currentWithSlash = uri.EndPathWithSlash();
        var baseDomains = domainsAndUris.Where(d => IsBaseOf(d, currentWithSlash)).ToList();

        // if none matches, try again without the port
        // ie current is www.example.com:1234/foo/bar, look for domain www.example.com
        var currentWithoutPort = currentWithSlash.WithoutPort();
        if (baseDomains.Count == 0)
            baseDomains = domainsAndUris.Where(d => IsBaseOf(d, currentWithoutPort)).ToList();

        return baseDomains;
    }

    private IReadOnlyCollection<DomainAndUri> SelectByCulture(IReadOnlyCollection<DomainAndUri> domainsAndUris, string culture, string defaultCulture) {

        // we try our best to match cultures, but may end with a bogus domain

        if (culture != null) // try the supplied culture
        {
            var cultureDomains = domainsAndUris.Where(x => x.Culture.InvariantEquals(culture)).ToList();
            if (cultureDomains.Count > 0) return cultureDomains;
        }

        if (defaultCulture != null) // try the defaultCulture culture
        {
            var cultureDomains = domainsAndUris.Where(x => x.Culture.InvariantEquals(defaultCulture)).ToList();
            if (cultureDomains.Count > 0) return cultureDomains;
        }

        return null;
    }

    private DomainAndUri GetByCulture(IReadOnlyCollection<DomainAndUri> domainsAndUris, string culture, string defaultCulture) {

        DomainAndUri domainAndUri;

        // we try our best to match cultures, but may end with a bogus domain

        if (culture != null) // try the supplied culture
        {
            domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.InvariantEquals(culture));
            if (domainAndUri != null) return domainAndUri;
        }

        if (defaultCulture != null) // try the defaultCulture culture
        {
            domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.InvariantEquals(defaultCulture));
            if (domainAndUri != null) return domainAndUri;
        }

        return domainsAndUris.First(); // what else?

    }

}