using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Spa.Models.Navigation;

/// <summary>
/// Class representing an item in the navigation.
/// </summary>
public class SpaNavigationItem : ISpaNavigationItem {

    #region Properties

    /// <summary>
    /// Gets or sets the ID of the item.
    /// </summary>
    [JsonProperty("id")]
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the title of the item.
    /// </summary>
    [JsonProperty("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the URL of the item.
    /// </summary>
    [JsonProperty("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Gets or sets the parent ID of the item.
    /// </summary>
    [JsonProperty("parentId")]
    public required int ParentId { get; init; }

    /// <summary>
    /// Gets or sets the template of the item.
    /// </summary>
    [JsonProperty("template")]
    public required string? Template { get; init; }

    /// <summary>
    /// Gets or sets the culture of the item.
    /// </summary>
    [JsonProperty("culture")]
    public required string? Culture { get; init; }

    /// <summary>
    /// Gets or sets whether the item has any children.
    /// </summary>
    [JsonProperty("hasChildren")]
    public required bool HasChildren { get; init; }

    /// <summary>
    /// Gets or sets whether the item is visible.
    /// </summary>
    [JsonProperty("isVisible")]
    public required bool IsVisible { get; init; }

    /// <summary>
    /// Gets or sets the children of the item.
    /// </summary>
    [JsonProperty("children")]
    public required IReadOnlyList<ISpaNavigationItem> Children { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaNavigationItem"/> class. All properties must be set after initialization, as they are required.
    /// </summary>
    public SpaNavigationItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaNavigationItem"/> class by copying values from an existing
    /// navigation item.
    /// </summary>
    /// <remarks>Copies property values from <paramref name="existing"/>, including the <c>Children</c>
    /// reference.</remarks>
    /// <param name="existing">Navigation item to copy values from.</param>
    [SetsRequiredMembers]
    public SpaNavigationItem(ISpaNavigationItem existing) {
        Id = existing.Id;
        Title = existing.Title;
        Url = existing.Url;
        ParentId = existing.ParentId;
        Template = existing.Template;
        Culture = existing.Culture;
        HasChildren = existing.HasChildren;
        IsVisible = existing.IsVisible;
        Children = existing.Children;
    }

    #endregion

}