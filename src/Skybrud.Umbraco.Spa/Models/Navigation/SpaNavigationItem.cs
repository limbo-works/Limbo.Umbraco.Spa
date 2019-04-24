using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Models.Navigation {

    public class SpaNavigationItem {

        #region Properties

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty("isVisible")]
        public bool IsVisible { get; set; }

        [JsonProperty("children")]
        public SpaNavigationItem[] Children { get; set; }

        #endregion

        #region Constructors

        protected SpaNavigationItem(IPublishedContent content, int levels = 1, int levelcount = 1) {

            IPublishedContent[] children = content.Children(x => x.TemplateId > 0 && x.IsVisible()).ToArray();

            Id = content.Id;
            Title = content.Name;
            Url = content.Url;
            ParentId = content.Parent?.Id ?? -1;
            Template = content.GetTemplateAlias();
            Culture = content.GetCultureInfo().Name;
            HasChildren = children.Any();
            IsVisible = content.Value<bool>(Constants.Conventions.Content.NaviHide) == false;
            Children = children.Any() && levels > levelcount ? children.Select(x => GetItem(x, levels)).ToArray() : new SpaNavigationItem[0];

        }

        #endregion

        #region Static methods

        public static SpaNavigationItem GetItem(IPublishedContent content, int levels = 1, int levelcount = 1) {
            return content == null ? null : new SpaNavigationItem(content, levels, levelcount);
        }

        public static IEnumerable<SpaNavigationItem> GetItems(IEnumerable<IPublishedContent> content, int levels = 1, int levelcount = 1) {
            if (content == null) return null;
            levelcount++;
            return content.Select(x => GetItem(x, levels, levelcount)).Where(x => x.IsVisible);
        }

        #endregion

    }

}