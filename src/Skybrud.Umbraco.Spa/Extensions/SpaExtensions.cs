using System.Globalization;
using System.Text.RegularExpressions;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Extensions {

    public static class SpaExtensions {

		public static bool IsPreviewUrl(this string url) {
			return url.GetPreviewId() > 0;
		}

		public static int GetPreviewId(this string url) {

            int nodeId;

			if (url.Contains("/umbraco/dialogs")) {
				
                int.TryParse(url.Split('=')[1], out nodeId);

            } else {

                Match match = Regex.Match(url.Split('?')[0], "^/([0-9]+)\\.aspx$");
			    return match.Success ? match.Groups[1].Value.ToInt32() : 0;

            }

			return nodeId;

        }

        public static CultureInfo GetCultureInfo(this IPublishedContent content) {
            return CultureInfo.GetCultureInfo(content.GetCultureFromDomains());
        }

    }

}