using System;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaLinkItem {

        [JsonProperty("url")]
        public string Url { get; }

        [JsonProperty("target")]
        public string Target { get; }
        
        protected SpaLinkItem(string url) {
            Url = url;
        }

        protected SpaLinkItem(IPublishedContent content) {
            Url = content.Url;
        }

        public static SpaLinkItem GetFromContent(IPublishedContent content) {
            return content == null ? null : new SpaLinkItem(content);
        }

        public static SpaLinkItem GetFromUrl(string url) {
            return String.IsNullOrWhiteSpace(url) ? null : new SpaLinkItem(url);
        }

    }

}