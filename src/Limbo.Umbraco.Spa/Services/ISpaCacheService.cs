namespace Limbo.Umbraco.Spa.Services;

/// <summary>
/// Interface describing a service for handling caching related to the SPA.
/// </summary>
public interface ISpaCacheService {

    /// <summary>
    /// Clears all caches related to the SPA.
    /// </summary>
    void ClearAll();

}