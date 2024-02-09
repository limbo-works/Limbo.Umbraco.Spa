namespace Limbo.Umbraco.Spa.Configuration;

/// <summary>
/// Enum class representing the query string mode. Default is <see cref="Auto"/>.
/// </summary>
public enum SpaQueryStringMode {

    /// <summary>
    /// Indicates that the SPA package should use <see cref="Encode"/>, but fall back to <see cref="Legacy"/> if
    /// the <c>query</c> parameter isn't found.
    /// </summary>
    Auto,

    /// <summary>
    /// Indicates that the SPA package should use the query string of the SPA API request (SPA specific parameters
    /// excluded).
    /// </summary>
    Legacy,

    /// <summary>
    /// Indciates that the SPA pacakge should look for a <c>query</c> parameter in the query string of the SPA API.
    /// The value of the parameter is an URL encoded version of the client-side query string.
    /// </summary>
    Encode

}