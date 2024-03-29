﻿using Limbo.Umbraco.Spa.Configuration;
using Limbo.Umbraco.Spa.Factories;
using Limbo.Umbraco.Spa.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Limbo.Umbraco.Spa;

/// <summary>
/// Class used for handling the DI dependencies of the <see cref="SpaRequestHelper"/> class.
/// </summary>
public class SpaRequestHelperDependencies {

    private readonly IOptions<SpaConfiguration> _spaConfiguration;

    #region Properties

    /// <summary>
    /// Gets a reference to the current environment.
    /// </summary>
    public IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Gets a reference to the current Umbraco context accessor.
    /// </summary>
    public IUmbracoContextAccessor UmbracoContextAccessor { get; }

    /// <summary>
    /// Gets a reference to Umbraco's service context.
    /// </summary>
    public ServiceContext Services { get; }

    /// <summary>
    /// Gets a reference to Umbraco's app caches.
    /// </summary>
    public AppCaches AppCaches { get; }

    /// <summary>
    /// Gets a reference to the redirects service.
    /// </summary>
    public IRedirectsService RedirectsService { get; }

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
    public SpaConfiguration Configuration => _spaConfiguration.Value;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance based on the specified dependencies.
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="umbracoContextAccessor"></param>
    /// <param name="serviceContext"></param>
    /// <param name="appCaches"></param>
    /// <param name="redirectsService"></param>
    /// <param name="logger"></param>
    /// <param name="variationContextAccessor"></param>
    /// <param name="publishedSnapshotAccessor"></param>
    /// <param name="domainRepository"></param>
    /// <param name="contentFactory"></param>
    /// <param name="spaConfiguration"></param>
    public SpaRequestHelperDependencies(IWebHostEnvironment environment, IUmbracoContextAccessor umbracoContextAccessor,
        ServiceContext serviceContext, AppCaches appCaches, ILogger<SpaRequestHelper> logger,
        IRedirectsService redirectsService, IVariationContextAccessor variationContextAccessor,
        IPublishedSnapshotAccessor publishedSnapshotAccessor, SpaDomainRepository domainRepository,
        ISpaContentFactory contentFactory, IOptions<SpaConfiguration> spaConfiguration) {
        _spaConfiguration = spaConfiguration;
        Environment = environment;
        UmbracoContextAccessor = umbracoContextAccessor;
        Services = serviceContext;
        AppCaches = appCaches;
        RedirectsService = redirectsService;
        Logger = logger;
        VariationContextAccessor = variationContextAccessor;
        PublishedSnapshotAccessor = publishedSnapshotAccessor;
        DomainRepository = domainRepository;
        ContentFactory = contentFactory;
    }

    #endregion

}