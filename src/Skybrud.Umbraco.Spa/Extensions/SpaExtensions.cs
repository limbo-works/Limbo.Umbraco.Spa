using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Core.Models.PublishedContent;

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

            // Umbraco 8 doesn't support getting the culture of an invariant node, so we're
            // currently just looking at the first segment of the URL
            //
            // This should be fixed in Umbraco 8.1: https://github.com/umbraco/Umbraco-CMS/issues/5170#issuecomment-485667483

            string cultureSegment = (content?.Url ?? string.Empty).Split('/').Skip(1).FirstOrDefault();

            return cultureSegment == "en" ? new CultureInfo("en-US") : new CultureInfo("da-DK");

        }

    }

}