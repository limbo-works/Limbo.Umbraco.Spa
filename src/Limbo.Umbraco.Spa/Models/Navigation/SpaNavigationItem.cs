using System.Collections.Generic;
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
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the item.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the URL of the item.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the parent ID of the item.
    /// </summary>
    [JsonProperty("parentId")]
    public int ParentId { get; set; }

    /// <summary>
    /// Gets or sets the template of the item.
    /// </summary>
    [JsonProperty("template")]
    public string Template { get; set; }

    /// <summary>
    /// Gets or sets the culture of the item.
    /// </summary>
    [JsonProperty("culture")]
    public string Culture { get; set; }

    /// <summary>
    /// Gets or sets whether the item has any children.
    /// </summary>
    [JsonProperty("hasChildren")]
    public bool HasChildren { get; set; }

    /// <summary>
    /// Gets or sets whether the item is visible.
    /// </summary>
    [JsonProperty("isVisible")]
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the children of the item.
    /// </summary>
    [JsonProperty("children")]
    public IReadOnlyList<ISpaNavigationItem> Children { get; set; }

    #endregion

}