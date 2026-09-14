using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.Spa.Constants;
using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Navigation;
using Skybrud.Essentials.Umbraco;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Factories;

/// <summary>
/// Class representing a basic a SPA item factory.
/// </summary>
public class SpaItemFactory : ISpaItemFactory {

    /// <summary>
    /// Creates a new navigation item for the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content to be converted into a navigation item.</param>
    /// <param name="request">The current SPA request.</param>
    /// <param name="maxLevels">The maximum level.</param>
    /// <param name="level">The current level.</param>
    /// <returns>An instance of <see cref="ISpaNavigationItem"/> if successful; otherwise, <c>false</c>.</returns>
    public virtual ISpaNavigationItem CreateNavigationItem(IPublishedContent content, SpaRequest request, int maxLevels, int level) {

        // Nothing to work on, nothing to return
        ArgumentNullException.ThrowIfNull(content);

        // Fetch all visible children that have a template
        IPublishedContent[] children = content.Children(x => x.TemplateId > 0 && x.IsVisible()).ToArray();

        // Convert the children to navigation items (if any, and max level isn't reached)
        IReadOnlyList<ISpaNavigationItem> childrenItems = [];
        if (children.Length > 0 && maxLevels > level) {
            childrenItems = children
                .Select(x => CreateNavigationItem(x, request, maxLevels, level + 1))
                .ToArray();
        }

        // Initialize a new item
        return new SpaNavigationItem {
            Id = content.Id,
            Title = content.Name,
            Url = content.Url(),
            ParentId = content.Parent()?.Id ?? -1,
            Template = content.GetTemplateAlias(), // TODO: don't use this as it may hit the database
            Culture = content.GetCultureInfo()?.Name,
            HasChildren = children.Length > 0,
            IsVisible = !content.Value<bool>(SkyConstants.Properties.UmbracoNaviHide),
            Children = childrenItems
        };

    }

}