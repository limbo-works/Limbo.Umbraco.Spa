using Limbo.Umbraco.Spa.Models;

namespace Limbo.Umbraco.Spa;

/// <summary>
/// Interface describing a way to access the current <see cref="SpaRequest"/>.
/// </summary>
public interface ISpaRequestAccessor {

    /// <summary>
    /// Gets a reference to the current SPA request, if any.
    /// </summary>
    SpaRequest Current { get; set; }

}