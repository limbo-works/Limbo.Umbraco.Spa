using System;
using Limbo.Umbraco.Spa.Models;
using Microsoft.AspNetCore.Http;

namespace Limbo.Umbraco.Spa {

    /// <summary>
    /// Class describing a way to access the current <see cref="SpaRequest"/>.
    /// </summary>
    public class SpaRequestAccessor : ISpaRequestAccessor {
        
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Properties

        /// <summary>
        /// Gets a reference to the current SPA request, if any.
        /// </summary>
        public SpaRequest Current {
            get {
                return _httpContextAccessor.HttpContext?.Items["SpaApiRequest"] as SpaRequest;
            }
            set {
                if (_httpContextAccessor.HttpContext == null) throw new Exception("Unable to access current HTTP context.");
                _httpContextAccessor.HttpContext.Items["SpaApiRequest"] = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="httpContextAccessor"/>.
        /// </summary>
        /// <param name="httpContextAccessor">The current <see cref="IHttpContextAccessor"/>.</param>
        public SpaRequestAccessor(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

    }

}