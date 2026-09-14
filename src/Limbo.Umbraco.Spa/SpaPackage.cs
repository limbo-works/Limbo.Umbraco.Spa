using System;
using System.Diagnostics;
using Umbraco.Cms.Core.Semver;

namespace Limbo.Umbraco.Spa;

/// <summary>
/// Static class with various information and constants about the package.
/// </summary>
public static class SpaPackage {

    /// <summary>
    /// Gets the alias of the package.
    /// </summary>
    public const string Alias = "Limbo.Umbraco.Spa";

    /// <summary>
    /// Gets the friendly name of the package.
    /// </summary>
    public const string Name = "Limbo SPA";

    /// <summary>
    /// Gets the version of the package.
    /// </summary>
    public static readonly Version Version = typeof(SpaPackage).Assembly.GetName().Version!;

    /// <summary>
    /// Gets the informational version of the package.
    /// </summary>
    public static readonly string InformationalVersion = FileVersionInfo
        .GetVersionInfo(typeof(SpaPackage).Assembly.Location).ProductVersion!
        .Split('+')[0];

    /// <summary>
    /// Gets the semantic version of the package.
    /// </summary>
    public static readonly SemVersion SemVersion = InformationalVersion;

    /// <summary>
    /// Gets the URL of the GitHub repository for this package.
    /// </summary>
    public const string GitHubUrl = "https://github.com/limbo-works/Limbo.Umbraco.Spa";

    /// <summary>
    /// Gets the URL of the issue tracker for this package.
    /// </summary>
    public const string IssuesUrl = "https://github.com/limbo-works/Limbo.Umbraco.Spa/issues";

    /// <summary>
    /// Gets the URL of the documentation for this package.
    /// </summary>
    public const string DocumentationUrl = "https://packages.limbo.works/limbo.umbraco.spa/v17/docs/";

}