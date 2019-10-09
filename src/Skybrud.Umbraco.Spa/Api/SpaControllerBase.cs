using System;
using System.Net.Http;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Spa.Attributes;
using Skybrud.Umbraco.Spa.Exceptions;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Flow;
using Skybrud.WebApi.Json;
using Umbraco.Core.Cache;
using Umbraco.Web.Composing;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Spa.Api {

    /// <summary>
    /// Base class for creating custom SPA API controllers.
    ///
    /// Server as a base class, most methods in this class have been marked as virtual, meaning your can override and tailor the controller to your needs.
    /// </summary>
    [JsonOnlyConfiguration]
    [AccessControlAllowOrigin]
    public abstract partial class SpaControllerBase : UmbracoApiController {

        #region Properties

        /// <summary>
        /// Gets a reference to the arguments of the current SPA request. 
        /// </summary>
        protected SpaRequestOptions Arguments => SpaRequest.Current.Arguments;

        /// <summary>
        /// Gets a reference to Umbraco's runtime cache.
        /// </summary>
        protected IAppPolicyCache RuntimeCache { get; }

        /// <summary>
        /// Gets a reference to the redirects service.
        /// </summary>
        protected IRedirectsService Redirects { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected SpaControllerBase() {
            RuntimeCache = AppCaches.RuntimeCache;
            Redirects = new RedirectsService(Current.ScopeProvider, Current.Services.DomainService, Logger);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="caches">A reference to the application caches.</param>
        /// <param name="redirects">A reference to the redirects service.</param>
        protected SpaControllerBase(AppCaches caches, IRedirectsService redirects) {
            RuntimeCache = caches.RuntimeCache;
            Redirects = redirects;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Virtual method defining the actions groups required for a successful SPA request. 
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <returns>An array of <see cref="SpaActionGroup"/>.</returns>
        protected virtual SpaActionGroup[] GetActionGroups(SpaRequest request) {

            // Declare an array of action groups to be executed. Each group features a lambda
            // expression to determine whether that particular group should be executed
            return new [] {

                // First group - should always be executed
                new SpaActionGroup(r => true, InitArguments, ReadFromCache),

                // Second group - not executed if we already have a data model (eg. from the cache)
                new SpaActionGroup(

                    // Continue if we don't have a model yet
                    r => r.DataModel == null,

                    InitSite,
                    PostInitSite,

                    PreContentLookup,
                    ContentLookup,
                    PostContentLookup,

                    PreSetupCulture,
                    SetupCulture,
                    PostSetupCulture,

                    InitSiteModel,

                    HandleNotFound,

                    InitContentModel,
                    InitDataModel,
                    InitNavigationModel,

                    PrePushToCache,
                    PushToCache

                ),

                // Third group - always executed
                new SpaActionGroup(r => true, RaisinInTheSausageEnd),

            };

        }

        /// <summary>
        /// Virtual method responsible for building the SPA response.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <returns>The response.</returns>
        protected virtual HttpResponseMessage GetResponse(SpaRequest request) {

            // Iterate through the different methods in the page flow
            foreach (SpaActionGroup group in GetActionGroups(request)) {

                // Should the group be executed?
                if (group.Run(request) == false) continue;

                // Iterate over and execute the actions of the group
                foreach (Action<SpaRequest> method in group.Actions) {

                    // Call the current flow method
                    try {
                        method(request);
                    } catch (Exception ex) {
                        throw new SpaActionException(request, group, method.Method, ex);
                    }

                    // Break the loop if we already have a response
                    if (request.Response != null) break;

                }

                // Break the loop if we already have a response
                if (request.Response != null) break;

            }

            // Generate a successful response
            if (request.Response == null && request.DataModel != null) {
                request.Response = CreateSpaResponse(request.ResponseStatusCode, request.DataModel);
            }

            // Generate a fallback error response
            return request.Response ?? ReturnError("Næh");

        }

        #endregion

    }

}