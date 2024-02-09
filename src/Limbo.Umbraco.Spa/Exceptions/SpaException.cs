using System;
using Limbo.Umbraco.Spa.Models;

namespace Limbo.Umbraco.Spa.Exceptions;

/// <summary>
/// Class representing a basic exception triggered while processing a <see cref="SpaRequest"/>.
/// </summary>
public class SpaException : Exception {

    #region Properties

    /// <summary>
    /// The current SPA request.
    /// </summary>
    public SpaRequest Request { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new expcetion based on the specified <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The current SPA request.</param>
    public SpaException(SpaRequest request) {
        Request = request;
    }

    /// <summary>
    /// Initializes a new expcetion based on the specified <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The current SPA request.</param>
    /// <param name="inner">The inner exception.</param>
    public SpaException(SpaRequest request, Exception inner) : base(null, inner) {
        Request = request;
    }

    /// <summary>
    /// Initializes a new expcetion based on the specified <paramref name="request"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="request">The current SPA request.</param>
    /// <param name="message">The message of the exception.</param>
    public SpaException(SpaRequest request, string message) : base(message) {
        Request = request;
    }

    #endregion

}