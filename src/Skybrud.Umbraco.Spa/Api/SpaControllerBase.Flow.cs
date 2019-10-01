using System;
using System.Net;
using Skybrud.Umbraco.Spa.Extensions;
using Skybrud.Umbraco.Spa.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Api {

    public abstract partial class SpaControllerBase {

        /// <summary>
        /// SPA request event method responsible for initializing the arguments of the request. Most arguments are
        /// determined directly from the request (eg. from the query string), while some information is determined
        /// while the request is processed.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitArguments(SpaRequest request) {

            // If "pageId" or "nodeId" exists, prefer content from that node
            if (Arguments.PageId > 0) {
	            IPublishedContent c = UmbracoContext.ContentCache.GetById(request.Arguments.PageId);
	            if (c != null) Arguments.Url = c.Url;
	        }

	        // Try get siteId from domain
	        if (Arguments.SiteId == -1 && !string.IsNullOrWhiteSpace(Arguments.HostName) && TryGetDomain(Arguments.HostName, out IDomain domain)) {
	            Arguments.SiteId = domain.RootContentId ?? -1;
	        }

	    }

        /// <summary>
        /// Virtual method responsible for finding the <see cref="IPublishedContent"/> representing the site node.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        protected virtual void InitSite(SpaRequest request) {

            // Get a reference to the site node
            request.Site = UmbracoContext.ContentCache.GetById(Arguments.SiteId);

            // Trigger an error response if the site node wasn't found (NULL means OK)
            if (request.Site == null) request.Response = ReturnError("Unable to determine site node from request.");

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

                // Get the ID from the request
                int id = request.Url.GetPreviewId();

                // Get a reference to the current page (fetched regardless of "parts" as the URL determines the culture)
                request.Content = global::Umbraco.Web.Composing.Current.UmbracoContext.ContentCache.GetById(true, id);

            } else {

                // Get a reference to the current page (fetched regardless of "parts" as the URL determines the culture)
                request.Content = GetContentFromInput(request.Site, Arguments.PageId, Arguments.Url);

            }

            // Handle "umbracoInternalRedirectId" when present
            if (request.Content != null && request.Content.HasValue(Constants.Conventions.Content.InternalRedirectId)) {
                request.Content = Umbraco.Content(Constants.Conventions.Content.InternalRedirectId);
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
            if (request.Culture != null) return;
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
            
            // Make sure to set the status as 404
            request.ResponseStatusCode = HttpStatusCode.NotFound;

            // Set "content" to the not found page
            request.Content = request.SiteModel?.NotFoundPage;

        }

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

            // Populate the data model with the content model if already present in the request
            if (request.ContentModel != null) {
                request.DataModel.Content = request.ContentModel;
                return;
            }

            switch (request.Content) {

                case PublishedContentModel content:
                    request.DataModel.Content = request.ContentModel = new SpaContentModel(content);
                    break;

            }

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
            if (Arguments.EnableCaching == false) return;

            // Attempt to get the data model from the runtime cache
            request.DataModel = RuntimeCache.Get(Arguments.CacheKey) as SpaDataModel;

            // Did we get a model?
            if (request.DataModel == null) return;

            // Update the data model with properties specific to this request
            request.DataModel.ExecuteTimeMs = request.Stopwatch.ElapsedMilliseconds;
            request.DataModel.IsCached = true;

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

            if (Arguments.EnableCaching == false) return;

            RuntimeCache.Insert(Arguments.CacheKey, () => request.DataModel, TimeSpan.FromSeconds(60));

        }
        
        protected virtual void RaisinInTheSausageEnd(SpaRequest request) { }

    }

}