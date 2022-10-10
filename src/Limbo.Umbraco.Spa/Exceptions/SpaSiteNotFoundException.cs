using Limbo.Umbraco.Spa.Models;

namespace Limbo.Umbraco.Spa.Exceptions {

    /// <summary>
    /// Exception class for when the SPA can't determine the current site.
    /// </summary>
    public class SpaSiteNotFoundException : SpaException {

        #region Constructors

        /// <summary>
        /// Initializes a new exception based on the specified <paramref name="request"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <param name="message">The message of the exception.</param>
        public SpaSiteNotFoundException(SpaRequest request, string message) : base(request, message) {

        }

        #endregion

    }

}