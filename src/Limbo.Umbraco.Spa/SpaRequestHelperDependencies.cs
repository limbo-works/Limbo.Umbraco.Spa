using Limbo.Umbraco.Spa.Configuration;
using Limbo.Umbraco.Spa.Factories;
using Limbo.Umbraco.Spa.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
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
    /// Gets a reference to Umbraco's logger.
    /// </summary>
    internal ILogger<SpaRequestHelper> Logger { get; }

    /// <summary>
    /// Gets a reference to the current environment.
    /// </summary>
    public IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Gets a reference to Umbraco's service context.
    /// </summary>
    public ServiceContext UmbracoServiceContext { get; }

    /// <summary>
    /// Gets a reference to the current domain service.
    /// </summary>
    public IDomainService DomainService { get; }

    /// <summary>
    /// Gets a reference to the current Umbraco context accessor.
    /// </summary>
    public IUmbracoContextAccessor UmbracoContextAccessor { get; }

    /// <summary>
    /// Gets a reference to Umbraco's app caches.
    /// </summary>
    public AppCaches AppCaches { get; }

    /// <summary>
    /// Gets a reference to Umbraco's document URL service.
    /// </summary>
    public IDocumentUrlService DocumentUrlService { get; }

    /// <summary>
    /// Gets a reference to Umbraco's redirect URL service.
    /// </summary>
    public IRedirectUrlService RedirectUrlService { get; }

    /// <summary>
    /// Gets a reference to the redirects service.
    /// </summary>
    public IRedirectsService RedirectsService { get; }

    /// <summary>
    /// Gets a reference to the current <see cref="IVariationContextAccessor"/>.
    /// </summary>
    public IVariationContextAccessor VariationContextAccessor { get; }

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
    /// <param name="logger"></param>
    /// <param name="environment"></param>
    /// <param name="redirectUrlService"></param>
    /// <param name="umbracoContextAccessor"></param>
    /// <param name="appCaches"></param>
    /// <param name="umbracoServiceContext"></param>
    /// <param name="domainService"></param>
    /// <param name="documentUrlService"></param>
    /// <param name="redirectsService"></param>
    /// <param name="variationContextAccessor"></param>
    /// <param name="domainRepository"></param>
    /// <param name="contentFactory"></param>
    /// <param name="spaConfiguration"></param>
    public SpaRequestHelperDependencies(

        // .NET dependencies
        ILogger<SpaRequestHelper> logger,
        IWebHostEnvironment environment,

        // Umbraco dependencies
        ServiceContext umbracoServiceContext,
        IDomainService domainService,
        IDocumentUrlService documentUrlService,
        IRedirectUrlService redirectUrlService,
        IUmbracoContextAccessor umbracoContextAccessor,
        IVariationContextAccessor variationContextAccessor,
        AppCaches appCaches,

        // Other
        IRedirectsService redirectsService,

        // SPA dependencies
        IOptions<SpaConfiguration> spaConfiguration,
        ISpaContentFactory contentFactory,
        SpaDomainRepository domainRepository

    ) {

        Logger = logger;
        Environment = environment;
        UmbracoServiceContext = umbracoServiceContext;
        DomainService = domainService;
        DocumentUrlService = documentUrlService;
        RedirectUrlService = redirectUrlService;
        UmbracoContextAccessor = umbracoContextAccessor;
        VariationContextAccessor = variationContextAccessor;
        AppCaches = appCaches;
        RedirectsService = redirectsService;
        _spaConfiguration = spaConfiguration;
        ContentFactory = contentFactory;
        DomainRepository = domainRepository;

    }

    #endregion

}