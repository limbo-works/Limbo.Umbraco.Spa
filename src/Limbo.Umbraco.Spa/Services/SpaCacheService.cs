using System;
using Umbraco.Cms.Core.Cache;

namespace Limbo.Umbraco.Spa.Services {

    /// <summary>
    /// Base implementation of a cache service for the SPA.
    /// </summary>
    public class SpaCacheService : ISpaCacheService {

        private readonly IAppPolicyCache _runtimeCache;

        /// <summary>
        /// Initializes a new instanced based on the specified <paramref name="caches"/>.
        /// </summary>
        /// <param name="caches">A reference to the Umbraco app caches.</param>
        public SpaCacheService(AppCaches caches) {
            _runtimeCache = caches.RuntimeCache;
        }

        /// <summary>
        /// Clears all caches related to the SPA.
        /// </summary>
        public void ClearAll() {

            // Change the content GUID so the frontend knows the content has been updated
            SpaEnvironment.ContentGuid = Guid.NewGuid();

            // Clear the micro cache
            _runtimeCache.ClearByKey(SpaConstants.CachePrefix);

        }


    }

}