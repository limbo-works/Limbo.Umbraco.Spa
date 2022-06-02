using System.Collections.Generic;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Spa.Models.Navigation {

    /// <summary>
    /// Interface describing an item in the navigation.
    /// </summary>
    public interface ISpaNavigationItem {

        #region Properties

        /// <summary>
        /// Gets the ID of the item.
        /// </summary>
        [JsonProperty("id")]
        int Id { get; }

        /// <summary>
        /// Gets the title of the item.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; }

        /// <summary>
        /// Gets or sets the URL of the item.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets or sets the parent ID of the item.
        /// </summary>
        [JsonProperty("parentId")]
        public int ParentId { get; }

        /// <summary>
        /// Gets or sets the template of the item.
        /// </summary>
        [JsonProperty("template")]
        public string Template { get; }

        /// <summary>
        /// Gets or sets the culture of the item.
        /// </summary>
        [JsonProperty("culture")]
        public string Culture { get; }

        /// <summary>
        /// Gets or sets whether the item has any children.
        /// </summary>
        [JsonProperty("hasChildren")]
        public bool HasChildren { get; }

        /// <summary>
        /// Gets or sets whether the item is visible.
        /// </summary>
        [JsonProperty("isVisible")]
        public bool IsVisible { get; }

        /// <summary>
        /// Gets or sets the children of the item.
        /// </summary>
        [JsonProperty("children")]
        public IReadOnlyList<ISpaNavigationItem> Children { get; }

        #endregion

    }

}