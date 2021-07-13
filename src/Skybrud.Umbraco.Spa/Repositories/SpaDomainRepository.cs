using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Skybrud.Umbraco.Spa.Models;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Spa.Repositories {
    
    public class SpaDomainRepository {
        
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly ISiteDomainHelper _siteDomainHelper;

        public SpaDomainRepository(IPublishedSnapshotAccessor publishedSnapshotAccessor, ISiteDomainHelper siteDomainHelper) {
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _siteDomainHelper = siteDomainHelper;
        }

        public DomainAndUri DomainForNode(int nodeId, Uri current, string culture = null) {

            // be safe
            if (nodeId <= 0) return null;

            // get the domains on that node
            var domains = _publishedSnapshotAccessor.PublishedSnapshot.Domains.GetAssigned(nodeId).ToArray();

            // none?
            if (domains.Length == 0) return null;

            // else filter
            // it could be that none apply (due to culture)
            return SelectDomain(domains, current, culture, _publishedSnapshotAccessor.PublishedSnapshot.Domains.DefaultCulture, _siteDomainHelper.MapDomain);

        }

        public DomainAndUri DomainForNode(IPublishedContent content, Uri current, string culture = null) {

            while (content != null) {

                DomainAndUri domain = DomainForNode(content.Id, current, culture);
                if (domain != null) return domain;

                content = content.Parent;

            }

            return null;

        }

        public bool FindDomain(SpaRequest request, Uri uri) {

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

                IPublishedContent c = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(request.Arguments.PageId);

                if (c != null) {
                    request.Domain = DomainForNode(c, null, request.Arguments.Culture);
                    if (request.Domain != null) {
                        request.CultureInfo = request.Domain.Culture;
                        return true;
                    }
                }

                // TODO: Should we return "false" here if no domains are found?

            }

            var domainsCache = _publishedSnapshotAccessor.PublishedSnapshot.Domains;
            var domains = domainsCache.GetAll(includeWildcards: false).ToList();

            // determines whether a domain corresponds to a published document, since some
            // domains may exist but on a document that has been unpublished - as a whole - or
            // that is not published for the domain's culture - in which case the domain does
            // not apply
            bool IsPublishedContentDomain(Domain domain) {

                // just get it from content cache - optimize there, not here
                var domainDocument = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(domain.ContentId);

                // not published - at all
                if (domainDocument == null)
                    return false;

                // invariant - always published
                if (!domainDocument.ContentType.VariesByCulture())
                    return true;

                // variant, ensure that the culture corresponding to the domain's language is published
                return domainDocument.Cultures.ContainsKey(domain.Culture.Name);

            }

            domains = domains.Where(IsPublishedContentDomain).ToList();

            var defaultCulture = domainsCache.DefaultCulture;

            // try to find a domain matching the current request
            var domainAndUri = SelectDomain(domains, uri, defaultCulture: defaultCulture);

            // handle domain - always has a contentId and a culture
            if (domainAndUri != null) {
                request.Domain = domainAndUri;
                request.CultureInfo = domainAndUri.Culture;

            } else {
                request.CultureInfo = defaultCulture == null ? CultureInfo.CurrentUICulture : new CultureInfo(defaultCulture);
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
            culture = culture.NullOrWhiteSpaceAsNull();
            defaultCulture = defaultCulture.NullOrWhiteSpaceAsNull();

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
                var cultureDomains = domainsAndUris.Where(x => x.Culture.Name.InvariantEquals(culture)).ToList();
                if (cultureDomains.Count > 0) return cultureDomains;
            }

            if (defaultCulture != null) // try the defaultCulture culture
            {
                var cultureDomains = domainsAndUris.Where(x => x.Culture.Name.InvariantEquals(defaultCulture)).ToList();
                if (cultureDomains.Count > 0) return cultureDomains;
            }

            return null;
        }

        private DomainAndUri GetByCulture(IReadOnlyCollection<DomainAndUri> domainsAndUris, string culture, string defaultCulture) {

            DomainAndUri domainAndUri;

            // we try our best to match cultures, but may end with a bogus domain

            if (culture != null) // try the supplied culture
            {
                domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.Name.InvariantEquals(culture));
                if (domainAndUri != null) return domainAndUri;
            }

            if (defaultCulture != null) // try the defaultCulture culture
            {
                domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.Name.InvariantEquals(defaultCulture));
                if (domainAndUri != null) return domainAndUri;
            }

            return domainsAndUris.First(); // what else?

        }

    }

}