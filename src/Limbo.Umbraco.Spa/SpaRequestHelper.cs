using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Limbo.Umbraco.Spa.Configuration;
using Limbo.Umbraco.Spa.Exceptions;
using Limbo.Umbraco.Spa.Factories;
using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Flow;
using Limbo.Umbraco.Spa.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.AspNetCore;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa;

/// <summary>
/// Helper class used for handling a SPA request.
/// </summary>
public partial class SpaRequestHelper {

    #region Properties

    /// <summary>
    /// Gets a reference to the current environment.
    /// </summary>
    public IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Gets a reference to the current Umbraco context accessor.
    /// </summary>
    protected IUmbracoContextAccessor UmbracoContextAccessor { get; }

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
    /// Gets a reference to Umbraco's logger.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Gets a reference to the current <see cref="IVariationContextAccessor"/>.
    /// </summary>
    public IVariationContextAccessor VariationContextAccessor { get; }

    /// <summary>
    /// Gets a reference to the current published snapshot accessor.
    /// </summary>
    public IPublishedSnapshotAccessor PublishedSnapshotAccessor { get; }

    /// <summary>
    /// Gets a reference to the current published snapshot.
    /// </summary>
    public SpaDomainRepository DomainRepository { get; }

    /// <summary>
    /// Gets a reference to the current SPA content factory.
    /// </summary>
    public ISpaContentFactory ContentFactory { get; }

    /// <summary>
    /// Gets a reference to the <see cref="SpaConfiguration"/>.
    /// </summary>
    public SpaConfiguration Configuration { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new helper instance.
    /// </summary>
    protected SpaRequestHelper(SpaRequestHelperDependencies dependencies) {

        // Set dependencies
        Environment = dependencies.Environment;
        UmbracoContextAccessor = dependencies.UmbracoContextAccessor;
        Services = dependencies.Services;
        AppCaches = dependencies.AppCaches;
        Logger = dependencies.Logger;
        RedirectsService = dependencies.RedirectsService;
        VariationContextAccessor = dependencies.VariationContextAccessor;
        PublishedSnapshotAccessor = dependencies.PublishedSnapshotAccessor;
        DomainRepository = dependencies.DomainRepository;
        ContentFactory = dependencies.ContentFactory;
        Configuration = dependencies.Configuration;

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
                _ => true,

                InitArguments,
                FindDomainAndCulture,
                UpdateArguments,

                ValidatePreviewAccess,

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
                HandleOutboundRedirects,

                PreInitModels,
                InitContentModel,
                InitDataModel,
                InitNavigationModel,
                InitCustomModels,
                PostInitModels,

                PrePushToCache,
                PushToCache

            ),

            // Third group - always executed
            new SpaActionGroup(_ => true, RaisinInTheSausageEnd)

        };

    }

    /// <summary>
    /// Virtual method responsible for building the SPA response.
    /// </summary>
    /// <param name="request">The current SPA request.</param>
    /// <returns>The response.</returns>
    public virtual ActionResult GetResponse(SpaRequest request) {

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

            // A bit of error handling
            request.Response = HandleGetResponseException(request, ex);

            // Return the response if present
            if (request.Response != null) return request.Response;

            throw;

        }

    }

    /// <summary>
    /// Method called when the <see cref="GetResponse"/> method results in an exception. The method can be used for
    /// custom error logging, as well as sending a different error message to the client.
    ///
    /// If the method returns a <see cref="HttpResponseMessage"/>, that response will be returned directly to the
    /// client. If the method returns <c>null</c>, the exception will bubble up through the request pipeline.
    ///
    /// By default, this method will write the exception to the Umbraco log, and if the solution is running i debug
    /// mode, and the Accept header of the request contains <c>text/html</c>, a user friendly HTML error message
    /// will be returned to the user.
    /// </summary>
    /// <param name="request">The current SPA request.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The response.</returns>
    protected virtual ActionResult HandleGetResponseException(SpaRequest request, Exception exception) {

        // Get a reference to the URI of the request
        Uri uri = request.HttpContext.Request.GetUri();

        if (exception is SpaActionException ex) {

            Logger.LogError(
                exception, "SPA request for scheme {Scheme}, domain {Domain} and URL {Url} failed. Action was {Action}.",
                uri.Scheme,
                uri.Host,
                uri.PathAndQuery,
                ex.MethodName
            );

        } else {

            Logger.LogError(
                exception, "SPA request for scheme {Scheme}, domain {Domain} and URL {Url} failed.",
                uri.Scheme,
                uri.Host,
                uri.PathAndQuery
            );

        }

        // If not in development mode, we don't overwrite the response
        if (!Environment.IsDevelopment()) return null;

        // Get the accept header from the current request
        string accept = request.HttpContext.Request.Headers["Accept"];

        if (Environment.IsDevelopment() && (accept?.Contains("text/html") ?? false)) {
            return ReturnHtmlError(request, exception);
        }

        return null;

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
        if (url.Contains("/umbraco/preview/") && int.TryParse(url.Split('=')[1], out result)) return true;

        // Find the ID using REGEX
        Match match = Regex.Match(url.Split('?')[0].TrimEnd('/'), "^/([0-9]+)\\.aspx$");
        result = match.Success ? match.Groups[1].Value.ToInt32() : 0;
        if (result > 0) return true;

        // Find the ID using REGEX
        match = Regex.Match(url.Split('?')[0].TrimEnd('/'), "^/([0-9]+)$");
        result = match.Success ? match.Groups[1].Value.ToInt32() : 0;

        return result > 0;

    }

    /// <summary>
    /// Protected method which can be used to enforcing a trailing slash at the end of <see cref="SpaRequestOptions.Url"/>.
    ///
    /// If the requested URL doesn't already end with a trailing slash, the user will then be redirected to the correct URL.
    /// </summary>
    /// <param name="request">The SPA request.</param>
    protected virtual void AddTrailingSlash(SpaRequest request) {

        if (request.Url == null || request.IsPreview) return;

        // Slit the URL so we don't look at the query string
        string[] url = request.Url.Split('?');

        // Return as the URL already ends with a trailing slash
        if (url[0].EndsWith("/")) return;

        // Append the trailing slash
        url[0] += "/";

        // Redirect the user to the correct URL
        request.Response = ReturnRedirect(request, string.Join("?", url));

    }

    /// <summary>
    /// Protected method which can be used to remove a trailing slash at the end of <see cref="SpaRequestOptions.Url"/>.
    ///
    /// If the requested URL ends with trailing slash, the user will then be redirected to the correct URL.
    /// </summary>
    /// <param name="request">The SPA request.</param>
    protected virtual void RemoveTrailingSlash(SpaRequest request) {

        if (request.IsPreview) return;

        // Slit the URL so we don't look at the query string
        string[] url = request.Url.Split('?');

        // Return as the URL doesn't end with a trailing slash
        if (!url[0].EndsWith("/")) return;

        // Remove the trailing slash
        url[0] = url[0][..^1];

        // Redirect the user to the correct URL
        request.Response = ReturnRedirect(request, string.Join("?", url));

    }

    private bool HasBackofficeIdentity(HttpContext context) {

        var cookieOptions = context.RequestServices
            .GetRequiredService<IOptionsSnapshot<CookieAuthenticationOptions>>()
            .Get(global::Umbraco.Cms.Core.Constants.Security.BackOfficeAuthenticationType);

        if (string.IsNullOrWhiteSpace(cookieOptions.Cookie.Name)) return false;
        if (!context.Request.Cookies.TryGetValue(cookieOptions.Cookie.Name, out string cookie)) return false;
        if (string.IsNullOrWhiteSpace(cookie)) return false;

        var unprotected = cookieOptions.TicketDataFormat.Unprotect(cookie);
        var backOfficeIdentity = unprotected?.Principal.GetUmbracoIdentity();
        return backOfficeIdentity is { IsAuthenticated: true };
    }

    /// <summary>
    /// Returns a unique key that identifies the current SPA API <paramref name="request"/>. The key will be used
    /// for storing and retrieving the <c>data</c> part in the runtime cache.
    /// </summary>
    /// <param name="request">The SPA request,</param>
    /// <returns>A unique cache key for <paramref name="request"/>.</returns>
    /// <remarks>The default cache key does not take members into account. If your site has a members area with
    /// login, an identifier for the member currently logged in should be a part of the cache key.</remarks>
    public virtual string GetCacheKey(SpaRequest request) {

        SpaRequestOptions options = request.Arguments;

        return $"{SpaConstants.CachePrefix}{options.PageId}-{options.SiteId}-{options.Url}-{options.IsPreview}-{string.Join(",", options.Parts ?? new List<SpaApiPart>())}-{options.Protocol}-{options.HostName}-{options.PortNumber}-{options.NavLevels}-{options.NavContext}";

    }

    #endregion

}