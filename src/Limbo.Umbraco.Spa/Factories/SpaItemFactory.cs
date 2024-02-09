using System;
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
        if (content == null) return null;

        // Initialize a new item
        SpaNavigationItem item = new();

        // Fetch all visible children that have a template
        IPublishedContent[] children = content.Children(x => x.TemplateId > 0 && x.IsVisible())?.ToArray() ?? Array.Empty<IPublishedContent>();

        // Update basic properties
        item.Id = content.Id;
        item.Title = content.Name;
        item.Url = content.Url();
        item.ParentId = content.Parent?.Id ?? -1;
        item.Template = content.GetTemplateAlias();
        item.Culture = content.GetCultureInfo().Name;
        item.HasChildren = children.Any();
        item.IsVisible = content.Value<bool>(SkyConstants.Properties.UmbracoNaviHide) == false;
        item.Children = Array.Empty<SpaNavigationItem>();

        // Append the item (if any, and max level isn't reached)
        if (children.Length > 0 && maxLevels > level) {
            item.Children = children
                .Select(x => CreateNavigationItem(x, request, maxLevels, level + 1))
                .ToArray();
        }

        return item;

    }

}