using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Skybrud.Umbraco.Spa.Factories;
using Skybrud.Umbraco.Spa.Repositories;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Skybrud.Umbraco.Spa {
    
    /// <summary>
    /// Class used for handling the DI dependencies of the <see cref="SpaRequestHelper"/> class.
    /// </summary>
    public class SpaRequestHelperDependencies {

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

        ///// <summary>
        ///// Gets a reference to the redirects service.
        ///// </summary>
        //protected IRedirectsService RedirectsService { get; }

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

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="umbracoContextAccessor"></param>
        /// <param name="serviceContext"></param>
        /// <param name="appCaches"></param>
        /// <param name="logger"></param>
        /// <param name="variationContextAccessor"></param>
        /// <param name="publishedSnapshotAccessor"></param>
        /// <param name="domainRepository"></param>
        /// <param name="contentFactory"></param>
        public SpaRequestHelperDependencies(IWebHostEnvironment environment, IUmbracoContextAccessor umbracoContextAccessor, ServiceContext serviceContext, AppCaches appCaches, ILogger<SpaRequestHelper> logger, IVariationContextAccessor variationContextAccessor, IPublishedSnapshotAccessor publishedSnapshotAccessor, SpaDomainRepository domainRepository, ISpaContentFactory contentFactory) {
            Environment = environment;
            UmbracoContextAccessor = umbracoContextAccessor;
            Services = serviceContext;
            AppCaches = appCaches;
            Logger = logger;
            VariationContextAccessor = variationContextAccessor;
            PublishedSnapshotAccessor = publishedSnapshotAccessor;
            DomainRepository = domainRepository;
            ContentFactory = contentFactory;
        }

        #endregion

    }

}