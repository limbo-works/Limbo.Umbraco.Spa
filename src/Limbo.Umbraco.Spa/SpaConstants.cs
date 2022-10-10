using System.Net;

// ReSharper disable InconsistentNaming

namespace Limbo.Umbraco.Spa {

    /// <summary>
    /// Static class with various constants related to the SPA.
    /// </summary>
    public static partial class SpaConstants {

        /// <summary>
        /// Gets the prefix used for micro caching.
        /// </summary>
        public const string CachePrefix = "SpaMicroCache-";

        /// <summary>
        /// Gets a reference to the <strong>418 I'm a teapot</strong> status code as it isn't available in <see cref="HttpStatusCode"/>.
        /// </summary>
        public const HttpStatusCode Teapot = (HttpStatusCode) 418;
    }

}