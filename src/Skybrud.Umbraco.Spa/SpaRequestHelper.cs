using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Spa.Exceptions;
using Skybrud.Umbraco.Spa.Json.Converters;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Flow;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Composing;

namespace Skybrud.Umbraco.Spa  {

    /// <summary>
    /// Helper class used for handling a SPA request.
    /// </summary>
    public partial class SpaRequestHelper {

        #region Properties

        /// <summary>
        /// Gets a reference to the current Umbraco context.
        /// </summary>
        protected UmbracoContext UmbracoContext { get; }

        /// <summary>
        /// Gets a reference to Umbraco's service context.
        /// </summary>
        protected ServiceContext Services { get; }

        /// <summary>
        /// Gets a reference to Umbraco's app caches.
        /// </summary>
        protected AppCaches AppCaches { get; }

        /// <summary>
        /// Gets a reference to the redirects service.
        /// </summary>
        protected IRedirectsService RedirectsService { get; }

        /// <summary>
        /// Gets or sets the JSON converter to be used when serializing the Umbraco grid.
        /// </summary>
        public SpaGridJsonConverterBase GridJsonConverter { get; set; }

        /// <summary>
        /// Gets a reference to Umbraco's logger.
        /// </summary>
        public ILogger Logger { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new helper instance.
        /// </summary>
        protected SpaRequestHelper() {
            UmbracoContext = Current.UmbracoContext;
            RedirectsService = Current.Factory.GetInstance<IRedirectsService>();
            Services = Current.Services;
            AppCaches = Current.AppCaches;
            Logger = Current.Logger;
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
                new SpaActionGroup(
                    r => true,
                    
                    InitArguments,
                    UpdateArguments,

                    ReadFromCache

                ),

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
                    InitCustomModels,

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
        public virtual HttpResponseMessage GetResponse(SpaRequest request) {

            try {
                
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

            } catch (Exception ex) {

                Logger.Error<SpaRequestHelper>(
                    ex, "SPA request for scheme {Scheme}, domain {Domain} and URL {Url} failed.",
                request.HttpContext.Request.Url?.Scheme,
                    request.HttpContext.Request.Url?.Host,
                    request.HttpContext.Request.RawUrl
                );

                if (request.HttpContext.IsDebuggingEnabled && (request.HttpContext.Request.AcceptTypes?.Contains("text/html") ?? false)) {
                    return ReturnHtmlError(request, ex);
                }

                throw;

            }

        }

        /// <summary>
        /// Gets the preview ID from the specified <paramref name="url"/>, or <c>0</c> if <paramref name="url"/> doesn't match an Umbraco preview URL.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="result">When this method returns, contains the preview ID of <paramref name="url"/>.</param>
        /// <returns><c>true</c> if <paramref name="url"/> matches a preview URL; otherwise <c>false</c>.</returns>
        public virtual bool TryGetPreviewId(string url, out int result) {

            // Shouldn't be null
            if (url == null) {
                result = 0;
                return false;
            }

            // Not sure the SPA API is hit for this URL
            if (url.Contains("/umbraco/dialogs") && int.TryParse(url.Split('=')[1], out result)) return true;

            // Find the ID using REGEX
            Match match = Regex.Match(url.Split('?')[0].TrimEnd('/'), "^/([0-9]+)\\.aspx$");
            result = match.Success ? match.Groups[1].Value.ToInt32() : 0;

            return result > 0;

        }

        #endregion

    }

}