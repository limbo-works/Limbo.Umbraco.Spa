#nullable enable

using System.Globalization;

namespace Limbo.Umbraco.Spa.Models;

/// <summary>
/// Class wrapping a <see cref="SpaDataModel"/> for caching purposes.
/// </summary>
public class SpaCachedModel {

    #region Properties

    /// <summary>
    /// Gets the SPA data model.
    /// </summary>
    public SpaDataModel Data { get; }

    /// <summary>
    /// Gets the culture. It's relevant to also cache the culture info so we'll be able
    /// to set the culture info of subsequent request when the model is read from the cache.
    /// </summary>
    public CultureInfo CultureInfo { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instanced based on the specified <paramref name="data"/> and <paramref name="cultureInfo"/>.
    /// </summary>
    /// <param name="data">The data model.</param>
    /// <param name="cultureInfo">The culture info of the request.</param>
    public SpaCachedModel(SpaDataModel data, CultureInfo cultureInfo) {
        Data = data;
        CultureInfo = cultureInfo;
    }

    #endregion

}