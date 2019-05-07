using System;
using System.Net.Http;
using Skybrud.Umbraco.Spa.Attributes;
using Skybrud.Umbraco.Spa.Exceptions;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Flow;
using Skybrud.WebApi.Json;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Spa.Api {

    [JsonOnlyConfiguration]
    [AccessControlAllowOrigin]
    public abstract partial class SpaControllerBase : UmbracoApiController {

        #region Properties

        /// <summary>
        /// Gets a reference to the arguments of the current SPA request. 
        /// </summary>
        protected SpaRequestOptions Arguments => SpaRequest.Current.Arguments;

        /// <summary>
        /// Gets a reference to Umbraco's content cache.
        /// </summary>
        protected IPublishedContentCache ContentCache => global::Umbraco.Web.Composing.Current.UmbracoContext.ContentCache;

        /// <summary>
        /// Gets a reference to Umbraco's media cache.
        /// </summary>
        protected IPublishedMediaCache MediaCache => global::Umbraco.Web.Composing.Current.UmbracoContext.MediaCache;

        /// <summary>
        /// Gets a reference to Umbraco's runtime cache.
        /// </summary>
        protected IAppPolicyCache RuntimeCache => Current.AppCaches.RuntimeCache;

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
        /// <returns></returns>
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