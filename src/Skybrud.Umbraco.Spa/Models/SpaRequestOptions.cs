using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Spa.Extensions;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaRequestOptions {

        #region Properties

        public int PageId { get; set; }

        public int SiteId { get; set; }

        public string Url { get; set; }

        public bool IsPreview { get; set; }

        public List<SpaApiPart> Parts { get; set; }

        public string Protocol { get; set; }

        public string HostName { get; set; }

        public int NavLevels { get; set; }

        public bool NavContext { get; set; }

        public string CacheKey => $"SpaMicroCache-{PageId}-{SiteId}-{Url}-{IsPreview}-{String.Join(",", Parts ?? new List<SpaApiPart>())}-{Protocol}-{HostName}-{NavLevels}-{NavContext}";

        public NameValueCollection QueryString { get; set; }

        public bool EnableCaching { get; set; }

        #endregion

        #region Constructors


        public SpaRequestOptions(SpaRequest request) : this(request.HttpContext) { }

        public SpaRequestOptions(HttpContextBase context) {

            HttpRequestBase r = context.Request;

            EnableCaching = context.IsDebuggingEnabled == false && r.QueryString["cache"] != "false";

            string appHost = r.QueryString["appHost"];

            string appProtocol = r.QueryString["appProtocol"];

            // Use the current URL as fallback for "appHost" and "appProtocol"
            HostName = String.IsNullOrWhiteSpace(appHost) ? r.Url?.Host : appHost;
            Protocol = String.IsNullOrWhiteSpace(appProtocol) ? r.Url?.Scheme : appProtocol;

            NavLevels = r.QueryString["navLevels"].ToInt32(1);
            NavContext = StringUtils.ParseBoolean(r.QueryString["navContext"]);

            Parts = GetParts(r.QueryString["parts"]);

            Url = r.QueryString["url"];

            SiteId = Math.Max(r.QueryString["siteId"].ToInt32(-1), r.QueryString["appSiteId"].ToInt32(-1));
            PageId = Math.Max(r.QueryString["pageId"].ToInt32(-1), r.QueryString["nodeId"].ToInt32(-1));

            QueryString = r.QueryString;

        }

        public SpaRequestOptions(int siteId) {
            SiteId = siteId;
            Url = "";
			IsPreview = false;
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url) {
            SiteId = siteId;
            Url = url;
			IsPreview = url.IsPreviewUrl();
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url, string parts) {
            SiteId = siteId;
			Url = string.IsNullOrWhiteSpace(url) ? "/" : url;
			IsPreview = Url.IsPreviewUrl();
			Parts = GetParts(parts);
        }

        #endregion
        
        #region Private methods

        /// <summary>
        /// Converts the specified string of <see cref="parts"/> to <see cref="List{SpaApiPart}"/>.
        /// </summary>
        /// <param name="parts">The string with the parts.</param>
        /// <returns>An an instance of <see cref="List{SpaApiPart}"/> containing each <see cref="SpaApiPart"/> specified in <see cref="parts"/>.</returns>
        private static List<SpaApiPart> GetParts(string parts = "") {
            
            // No parts means all parts
            if (String.IsNullOrWhiteSpace(parts)) return new List<SpaApiPart> { SpaApiPart.Content, SpaApiPart.Navigation, SpaApiPart.Site };

            List<SpaApiPart> temp = new List<SpaApiPart>();
            foreach (string item in parts.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                if (EnumUtils.TryParseEnum(item, out SpaApiPart part)) temp.Add(part);
            }

            return temp;

        }

        #endregion

    }

}