using System.Net;

namespace Limbo.Umbraco.Spa.Configuration {

    /// <summary>
    /// Class represrnting the global configuration of the SPA package.
    /// </summary>
    public class SpaConfiguration {

        /// <summary>
        /// Gets the query string mode. Possible values are:
        ///
        /// <ul>
        /// <li>
        ///     <strong>Auto</strong><br />
        ///     Looks for a <c>query</c> parameter (<see cref="SpaQueryStringMode.Encode"/>). If not found, uses
        ///     <see cref="SpaQueryStringMode.Legacy"/> as fallback.
        /// </li>
        /// <li>
        ///     <strong>Legacy</strong><br />
        ///     Uses the query string of the SPA API request, but filters out SPA specific parameters.
        /// </li>
        /// <li>
        ///     <strong>Encoded</strong><br />
        ///     Looks for a <c>query</c> parameter in the query string of the SPA API request holding the URL encoded client-side query string.
        /// </li>
        /// </ul>
        /// </summary>
        public SpaQueryStringMode QueryStringMode { get; private set; } = SpaQueryStringMode.Auto;

        /// <summary>
        /// Gets whether the helper should overwrite the status code of responses returned by the helper. Default is <c>true</c>.
        ///
        /// When enabled, status codes like <see cref="HttpStatusCode.MovedPermanently"/>,
        /// <see cref="HttpStatusCode.TemporaryRedirect"/> and <see cref="HttpStatusCode.NotFound"/> will be set to
        /// <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        public bool OverwriteStatusCodes { get; private set; } = true;

    }

}