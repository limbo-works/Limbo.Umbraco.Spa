using System;
using System.Collections.Generic;
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

        #endregion

        #region Constructors

        public SpaRequestOptions() { }

        public SpaRequestOptions(HttpRequest request) {

            string appHost = request.QueryString["appHost"];

            string appProtocol = request.QueryString["appProtocol"];

            // Use the current URL as fallback for "appHost" and "appProtocol"
            HostName = String.IsNullOrWhiteSpace(appHost) ? request.Url.Host : appHost;
            Protocol = String.IsNullOrWhiteSpace(appProtocol) ? request.Url.Scheme : appProtocol;

            NavLevels = request.QueryString["navLevels"].ToInt32(1);
            NavContext = StringUtils.ParseBoolean(request.QueryString["navContext"]);

            Parts = GetParts(request.QueryString["parys"]);

            Url = request.QueryString["url"];

            SiteId = Math.Max(request.QueryString["siteId"].ToInt32(-1), request.QueryString["appSiteId"].ToInt32(-1));
            PageId = Math.Max(request.QueryString["pageId"].ToInt32(-1), request.QueryString["nodeId"].ToInt32(-1));

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