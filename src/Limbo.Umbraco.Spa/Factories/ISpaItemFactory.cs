using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Navigation;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Spa.Factories;

/// <summary>
/// Interface describing a SPA item factory.
/// </summary>
public interface ISpaItemFactory {

    /// <summary>
    /// Creates a new navigation item for the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content to be converted into a navigation item.</param>
    /// <param name="request">The current SPA request.</param>
    /// <returns>An instance of <see cref="ISpaNavigationItem"/> if successful; otherwise, <c>false</c>.</returns>
    public ISpaNavigationItem CreateNavigationItem(IPublishedContent content, SpaRequest request) {
        return CreateNavigationItem(content, request, 1, 1);
    }

    /// <summary>
    /// Creates a new navigation item for the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content to be converted into a navigation item.</param>
    /// <param name="request">The current SPA request.</param>
    /// <param name="maxLevels">The maximum level.</param>
    /// <returns>An instance of <see cref="ISpaNavigationItem"/> if successful; otherwise, <c>false</c>.</returns>
    public ISpaNavigationItem CreateNavigationItem(IPublishedContent content, SpaRequest request, int maxLevels) {
        return CreateNavigationItem(content, request, maxLevels, 1);
    }

    /// <summary>
    /// Creates a new navigation item for the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content to be converted into a navigation item.</param>
    /// <param name="request">The current SPA request.</param>
    /// <param name="maxLevels">The maximum level.</param>
    /// <param name="level">The current level.</param>
    /// <returns>An instance of <see cref="ISpaNavigationItem"/> if successful; otherwise, <c>false</c>.</returns>
    ISpaNavigationItem CreateNavigationItem(IPublishedContent content, SpaRequest request, int maxLevels, int level);

}