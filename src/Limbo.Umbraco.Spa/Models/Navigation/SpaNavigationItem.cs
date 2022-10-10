using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.Spa.Constants;
using Newtonsoft.Json;
using Skybrud.Essentials.Umbraco;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Models.Navigation {

    /// <summary>
    /// Class representing an item in the navigation.
    /// </summary>
    public class SpaNavigationItem {

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
        public SpaNavigationItem[] Children { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> the item should be based on.</param>
        /// <param name="levels">The maximal level to be returned.</param>
        /// <param name="levelcount">The current level in the content tree.</param>
        protected SpaNavigationItem(IPublishedContent content, int levels = 1, int levelcount = 1) {

            IPublishedContent[] children = content.Children(x => x.TemplateId > 0 && x.IsVisible()).ToArray();

            Id = content.Id;
            Title = content.Name;
            Url = content.Url();
            ParentId = content.Parent?.Id ?? -1;
            Template = content.GetTemplateAlias();
            Culture = content.GetCultureInfo().Name;
            HasChildren = children.Any();
            IsVisible = content.Value<bool>(SkyConstants.Properties.UmbracoNaviHide) == false;
            Children = children.Any() && levels > levelcount ? children.Select(x => GetItem(x, levels, levelcount + 1)).ToArray() : new SpaNavigationItem[0];

        }

        #endregion

        #region Static methods

        /// <summary>
        /// Returns a new instance of <see cref="SpaNavigationItem"/> based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> the item should be based on.</param>
        /// <param name="levels">The maximal level to be returned.</param>
        /// <param name="levelcount">The current level in the content tree.</param>
        /// <returns>An instance of <see cref="SpaNavigationItem"/>.</returns>
        public static SpaNavigationItem GetItem(IPublishedContent content, int levels = 1, int levelcount = 1) {
            return content == null ? null : new SpaNavigationItem(content, levels, levelcount);
        }

        /// <summary>
        /// Returns a new collection of <see cref="SpaNavigationItem"/> based on the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">A collection of <see cref="IPublishedContent"/> the items should be based on.</param>
        /// <param name="levels">The maximal level to be returned.</param>
        /// <param name="levelcount">The current level in the content tree.</param>
        /// <returns>A collection of <see cref="SpaNavigationItem"/>.</returns>
        public static IEnumerable<SpaNavigationItem> GetItems(IEnumerable<IPublishedContent> content, int levels = 1, int levelcount = 1) {
            if (content == null) return null;
            levelcount++;
            return content.Select(x => GetItem(x, levels, levelcount)).Where(x => x.IsVisible);
        }

        #endregion

    }

}