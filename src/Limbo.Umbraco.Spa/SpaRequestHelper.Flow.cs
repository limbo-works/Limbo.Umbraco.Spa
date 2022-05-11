using System;
using System.Globalization;
using System.Net;
using Limbo.Umbraco.Spa.Constants;
using Limbo.Umbraco.Spa.Exceptions;
using Limbo.Umbraco.Spa.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa {

    public partial class SpaRequestHelper {

        /// <summary>
        /// SPA request event method responsible for initializing the arguments of the request.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitArguments(SpaRequest request) {
            request.Arguments = new SpaRequestOptions(request, this);
        }

        /// <summary>
        /// SPA request event method responsible for determining the domain and culture of <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void FindDomainAndCulture(SpaRequest request) {

            // Find the domain (sets the "Domain" and "CultureInfo" of "request")
            DomainRepository.FindDomain(request, request.Arguments.Uri);

            if (request.Arguments.PageId > 0) {

                // If a page ID was specifically specified for the request, it may mean that we're
                // in preview mode or that the "url" parameter isn't specified. In either case, we
                // need to find the assigned domains of the requested node (or it's ancestor) so we
                // can determine the sitenode

                // TODO: Look at the "siteId" parameter as well (may be relevant for virtual content etc.)
                
                IPublishedContent c = UmbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(request.Arguments.PageId);

                if (c != null) {
                    
                    request.Domain = DomainRepository.DomainForNode(c, null, request.Arguments.QueryString["culture"]);

                    if (request.Domain != null) {
                        request.CultureInfo = CultureInfo.GetCultureInfo(request.Domain.Culture);
                    }

                }


            }

            // Make sure to overwrite the variation context
            VariationContextAccessor.VariationContext = new VariationContext(request.CultureInfo.Name);

        }

        /// <summary>
        /// SPA request event method responsible for updating the arguments of the request. Most arguments are
        /// determined directly from the request (eg. from the query string), while some information is determined
        /// while the request is processed.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void UpdateArguments(SpaRequest request) {

            // If "pageId" exists, prefer content from that node
            if (request.Arguments.PageId > 0) {
                IPublishedContent c = UmbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(request.Arguments.PageId);
                if (c != null) request.Arguments.Url = c.Url();
            }

            // Check exit conditions
            if (request.Arguments.SiteId > 0) return;
            if (string.IsNullOrWhiteSpace(request.Arguments.HostName)) return;

            // Try get siteId from the domain
            if (!request.Arguments.IsDefaultPort && TryGetDomain(request.Arguments.HostName, out IDomain domain)) {
                // TODO: Should we set "request.Domain" here?
                request.Arguments.SiteId = domain.RootContentId ?? -1;
            }

            if (TryGetDomain(request.Arguments.HostName + ":" + request.Arguments.PortNumber, out domain)) {
                // TODO: Should we set "request.Domain" here?
                request.Arguments.SiteId = domain.RootContentId ?? -1;
            }

        }

        /// <summary>
        /// If preview mode is enabled, checks whether the user is authenticated in the Umbraco backoffice. If not,
        /// <see cref="SpaRequest.Response"/> will be set to a <see cref="HttpStatusCode.Unauthorized"/> error response.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void ValidatePreviewAccess(SpaRequest request) {

            if (!request.IsPreview) return;

            if (!HasBackofficeIdentity(request.HttpContext)) {
                request.Response = ReturnError(HttpStatusCode.Unauthorized, "User not authenticated.");
            }
            
        }

        /// <summary>
        /// Virtual method responsible for finding the <see cref="IPublishedContent"/> representing the site node.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitSite(SpaRequest request) {

            // Get a reference to the site node
            request.Site = UmbracoContextAccessor
                .GetRequiredUmbracoContext()
                .Content
                .GetById(request.Arguments.SiteId);

            // Throw an exception if we can't determine the site node
            if (request.Site == null) throw new SpaSiteNotFoundException(request, "Unable to determine site node from request.");

        }

        private void PostInitSite(SpaRequest request) {
            request.SiteId = request.Site?.Id ?? request.SiteId;
        }

        /// <summary>
        /// Virtual method called before the controller will attempt to look up the <see cref="IPublishedContent"/> representing the requested page.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void PreContentLookup(SpaRequest request) { }

        /// <summary>
        /// Virtual method for looking up the <see cref="IPublishedContent"/> representing the requested page.
        ///
        /// This method will automatically handle the <c>umbracoInternalRedirectId</c> property when present.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void ContentLookup(SpaRequest request) {

            // Return here as the content item already has been populated
            if (request.Content != null) return;

            // We should probably handle preview mode
            if (request.IsPreview) {

                // Get a reference to the current page (fetched regardless of "parts" as the URL determines the culture)
                request.Content = UmbracoContextAccessor
                    .GetRequiredUmbracoContext()
                    .Content.
                    GetById(true, request.Arguments.PageId);

            } else {

                // Get a reference to the current page (fetched regardless of "parts" as the URL determines the culture)
                request.Content = GetContentFromRequest(request);

            }

            // Handle "umbracoInternalRedirectId" when present
            if (request.Content != null && request.Content.HasValue(SkyConstants.Properties.UmbracoInternalRedirect)) {
                request.Content = request.Content.Value<IPublishedContent>(SkyConstants.Properties.UmbracoInternalRedirect);
            }

        }

        /// <summary>
        /// Virtual method called after the controller will attempt to look up the <see cref="IPublishedContent"/>
        /// representing the requested page.
        /// </summary>
        /// <example>
        /// This method can be used for creating custom internal redirects (showing another page than the one that was
        /// actually requested). In our typical site setup, we have a culture node with settings for that particular
        /// culture - eg. site name and similar. The culture node mostly a settings node, and as such it doesn't have
        /// any content. So when a user requests the culture node (eg. with an URL such as <c>/da/</c> or <c>/en/</c>),
        /// we set the current page as the frontpage of the culture instead:
        /// <code>
        /// protected override void PostContentLookup(SpaRequest request) {
        /// 
        ///     // Skip if we don't have a content item
        ///     if (request.Content == null) return;
        /// 
        ///     // If "content" matches the culture node, we get the content of the front page instead
        ///     if (request.Content.ContentType.Alias == SkyConstants.DocumentTypes.Culture) {
        ///         request.Content = request.Content.FirstChild(SkyConstants.DocumentTypes.FrontPage) ?? request.Content;
        ///     }
        /// 
        /// }
        /// </code>
        /// </example>
        /// <param name="request">The current SPA request.</param>
        protected virtual void PostContentLookup(SpaRequest request) { }

        /// <summary>
        /// Virtual method called before the controller will attempt to look up the <see cref="IPublishedContent"/> representing the culture node of the current request.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void PreSetupCulture(SpaRequest request) { }

        /// <summary>
        /// Virtual method for looking up the <see cref="IPublishedContent"/> representing the culture node of the current request.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void SetupCulture(SpaRequest request) {
            if (request.Site != null) return;
            request.Culture = request.Site;
        }

        /// <summary>
        /// Virtual method called after the controller will attempt to look up the <see cref="IPublishedContent"/> representing the culture node of the current request.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void PostSetupCulture(SpaRequest request) { }

        /// <summary>
        /// Populates the <see cref="SpaRequest.SiteModel"/> property based on <see cref="SpaRequest.Site"/> (and <see cref="SpaRequest.Culture"/> if present).
        /// 
        /// The site model is an instance of <see cref="SpaSiteModel"/> (or it may be a subclass thereof if <see cref="InitSiteModel"/> is overridden).
        /// </summary>
        /// <example>
        /// You use your own site model by overriding the method similar to as shown below. Notice that your site model must extend the <see cref="SpaSiteModel"/> class.
        /// <code>
        /// protected override void InitSiteModel(SpaRequest request) {
        /// 	if (request.Site == null) return;
        /// 	request.SiteModel = new VeggySiteModel(request.Site, request.Culture);
        /// }
        /// </code>
        /// </example>
        /// <param name="request">The current request.</param>
        protected virtual void InitSiteModel(SpaRequest request) {
            if (request.Site == null) return;
            request.SiteModel = new SpaSiteModel(request.Site, request.Culture);
        }

        /// <summary>
        /// Virtual method used for handling 404 error pages. This method will automatically set
        /// <see cref="SpaSiteModel.NotFoundPage"/> as the current page if present, and
        /// <see cref="SpaRequest.Content"/> hasn't already been populated.
        ///
        /// Notice that the <see cref="SpaSiteModel.NotFoundPage"/> property will not automatically be populated as it
        /// doesn't know how your site defines the 404 error page. You can set the property by creating your own site
        /// model that extends the <see cref="SpaSiteModel"/> class.
        /// </summary>
        /// <example>
        /// Example on how to populated the <see cref="SpaSiteModel.NotFoundPage"/> property:
        /// <code>
        /// public class MySiteModel : SpaSiteModel {
        /// 
        ///     public MySiteModel(IPublishedContent site) : this(site, null) { }
        /// 
        ///     public MySiteModel(IPublishedContent site, IPublishedContent culture) : base(site, culture) {
        ///         NotFoundPage = Culture.TypedContent("skyNotFoundPage");
        ///     }
        /// 
        /// }
        /// </code>
        /// </example>
        /// <param name="request">The current request.</param>
        protected virtual void HandleNotFound(SpaRequest request) {

            // Return now if we already have a content item
            if (request.Content != null) return;

            HandleInboundRedirects(request);

            // Make sure to set the status as 404
            request.ResponseStatusCode = HttpStatusCode.NotFound;

            // Set "content" to the not found page
            request.Content = request.SiteModel?.NotFoundPage;

        }

        /// <summary>
        /// Virtual method for handling inbound redirects.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns><c>true</c> if a redirect was found, otherwise <c>false</c>.</returns>
        protected virtual bool HandleInboundRedirects(SpaRequest request) {
            if (HandleSkybrudRedirect(request)) return true;
            if (HandleUmbracoRedirect(request)) return true;
            return false;
        }

        /// <summary>
        /// Virtual method for handling inbound redirects created by editors through Skybrud's redirects package.
        ///
        /// The redirects handled by this method are the ones retrived through the <c>IRedirectsService</c> service.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns><c>true</c> if a redirect was found, otherwise <c>false</c>.</returns>
        protected virtual bool HandleSkybrudRedirect(SpaRequest request) {

            return false;

            //// Look for a global Skybrud redirect
            //RedirectItem redirect = RedirectsService.GetRedirectByUrl(0, HttpUtility.UrlDecode(request.Url));

            //// If nothing is found at this point, look for a site specific Skybrud redirect
            //if (request.SiteId > 0 && redirect == null) {
            //    redirect = RedirectsService.GetRedirectByUrl(request.SiteId, HttpUtility.UrlDecode(request.Url));
            //}

            //if (redirect == null) return false;

            //// Return a redirect response based on the Skybrud redirect
            //request.Response = ReturnRedirect(request, redirect.LinkUrl, redirect.IsPermanent);

            //return true;

        }

        /// <summary>
        /// Virtual method for handling redirects created automatically by Umbraco when editors rename and move content. The redirects handled by this method are the ones retrived through the <see cref="IRedirectUrlService"/> service.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns><c>true</c> if a redirect was found, otherwise <c>false</c>.</returns>
        protected virtual bool HandleUmbracoRedirect(SpaRequest request) {

            // Look for a matching redirect
            IRedirectUrl umbRedirect = Services.RedirectUrlService.GetMostRecentRedirectUrl(request.SiteId + request.Url.TrimEnd('/'));
            if (umbRedirect == null) return false;

            // Get the destination page from the content cache
            IPublishedContent newContent = UmbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(umbRedirect.ContentId);
            if (newContent == null) return false;

            // Send a redirect response if a page was found
            request.Response = ReturnRedirect(request, newContent.Url(), true);
            return true;

        }

        /// <summary>
        /// Virtual method for handling outbound redirects - wich is redirects pointing from content in Umbraco to somewhere else.
        /// </summary>
        /// <param name="request">The current request.</param>
        protected virtual void HandleOutboundRedirects(SpaRequest request) {

            //// Get the outbound URL from the current page (if set)
            //OutboundRedirect redirect = request.Content?.GetOutboundRedirect();

            //// If the redirect is valid, we'll set the response to return a redirect
            //if (redirect != null && redirect.HasDestination) {
            //    request.Response = ReturnRedirect(request, redirect.Destination.Url, redirect.IsPermanent);
            //}

        }

        /// <summary>
        /// Virtual method executed right before <see cref="InitContentModel"/>.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void PreInitModels(SpaRequest request) { }

        /// <summary>
        /// Virtual method for initializing the <see cref="SpaContentModel"/> representing the requested page.
        ///
        /// The default implementation is able to detect models from ModelsBuilder, in which case it may not be
        /// neccessary to override this method. On the other hand, you can create your own implementation to get a
        /// better control of the model returned for a given (type of) page.
        /// </summary>
        /// <example>
        /// For more control over the returned models, you can override the method as shown below.
        /// <code>
        /// protected override void InitContentModel(SpaRequest request) {
        /// 
        ///     if (request.Content == null || request.Arguments.Parts.Contains(SpaApiPart.Content) == false) return;
        /// 
        ///     switch (request.Content.ContentType.Alias) {
        /// 
        ///         case "myArticle":
        ///             request.ContentModel = new MyArticle(request.Content);
        ///             break;
        ///         
        ///     }
        /// 
        /// }
        /// </code>
        /// </example>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitContentModel(SpaRequest request) {

            // Skip this part if the "content" part wasn't requested
            if (request.Arguments.Parts.Contains(SpaApiPart.Content) == false) return;
            
            // Initialize the new model
            request.ContentModel = ContentFactory.CreateContentModel(request.Content, new PublishedValueFallback(Services, VariationContextAccessor), request);

        }

        /// <summary>
        /// Virtual method responsible for initializing the navigation part of the response. The navigation model is
        /// only initialized if <see cref="SpaApiPart.Navigation"/> is present in the request arguments.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitNavigationModel(SpaRequest request) {

            // Skip this part if the "navigation" part wasn't requested
            if (request.Arguments.Parts.Contains(SpaApiPart.Navigation) == false) return;

            // Initialize the navigation model
            request.DataModel.Navigation = request.SiteModel.GetNavigation();

        }

        /// <summary>
        /// Virtual method responsible for initializing custom models/part of the response.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitCustomModels(SpaRequest request) { }

        /// <summary>
        /// Virtual method executed right after <see cref="InitContentModel"/>.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void PostInitModels(SpaRequest request) { }

        /// <summary>
        /// Virtual method responsible for initialing the main data model of the SPA response.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitDataModel(SpaRequest request) {
            request.DataModel = new SpaDataModel(request);
        }

        /// <summary>
        /// Virtual method responsible for reading from the micro cache. Micro caching is only enabled if
        /// <see cref="SpaRequestOptions.EnableCaching"/> is <c>true</c>.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void ReadFromCache(SpaRequest request) {

            // Skip if caching is disabled
            if (request.Arguments.EnableCaching == false) return;

            // Attempt to get the data model from the runtime cache
            request.DataModel = AppCaches.RuntimeCache.Get(request.Arguments.CacheKey) as SpaDataModel;

            // Did we get a model?
            if (request.DataModel == null) return;

            // Update the data model with properties specific to this request
            request.DataModel.ExecuteTimeMs = request.Stopwatch.ElapsedMilliseconds;
            request.DataModel.IsCached = true;

            // Update the status code based on the cached model (eg. 404)
            request.ResponseStatusCode = request.DataModel.Meta.StatusCode;


        }

        /// <summary>
        /// Virtual method executed right before the data model is pushed to the runtime cache.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        private void PrePushToCache(SpaRequest request) {

            if (request.DataModel == null) return;
            if (request.DataModel.ExecuteTimeMs >= 0) return;

            request.DataModel.ExecuteTimeMs = request.Stopwatch.ElapsedMilliseconds;

        }

        /// <summary>
        /// Virtual method responsible for pushing the data model to the micro cache.  Micro caching is only enabled if
        /// <see cref="SpaRequestOptions.EnableCaching"/> is <c>true</c>.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void PushToCache(SpaRequest request) {

            if (request.DataModel == null) return;

            if (request.Arguments.EnableCaching == false) return;

            AppCaches.RuntimeCache.Insert(request.Arguments.CacheKey, () => request.DataModel, TimeSpan.FromSeconds(60));

        }

        /// <summary>
        /// Virtual method that is executed at the end of the page cycle. It doesn't really do anything by default, but
        /// you can override it to customize the response before it is send to the client.
        ///
        /// Notice that this method is executed for both cached and non-cached responses.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void RaisinInTheSausageEnd(SpaRequest request) { }

    }

}