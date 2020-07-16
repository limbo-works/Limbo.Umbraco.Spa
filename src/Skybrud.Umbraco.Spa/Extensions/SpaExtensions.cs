using System.Globalization;
using System.Text.RegularExpressions;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Extensions {

    /// <summary>
    /// Static class with various extension methods for the SPA.
    /// </summary>
    public static class SpaExtensions {

        /// <summary>
        /// Returns whether the specified <paramref name="url"/> matches a preview URL.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns><c>true</c> if <paramref name="url"/> matches a preview URL, otherwise <c>false</c>.</returns>
		public static bool IsPreviewUrl(this string url) {
			return url.GetPreviewId() > 0;
		}

        /// <summary>
        /// Returns the numeric ID of the content item matching the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">A preview URL.</param>
        /// <returns>The numeric of a content item if <paramref name="url"/> matched a preview URL, otherwise <c>0</c>.</returns>
		public static int GetPreviewId(this string url) {

            int nodeId;

			if (url.Contains("/umbraco/dialogs")) {
				
                int.TryParse(url.Split('=')[1], out nodeId);

            } else {

                Match match = Regex.Match(url.Split('?')[0].TrimEnd('/'), "^/([0-9]+)\\.aspx$");
			    return match.Success ? match.Groups[1].Value.ToInt32() : 0;

            }

			return nodeId;

        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> of the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The content item to get the culture item for.</param>
        /// <returns>An instance of <see cref="CultureInfo"/>.</returns>
        /// <see>
        ///     <cref>https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.getcultureinfo</cref>
        /// </see>
        /// <remarks>
        /// <para>This extension method works by looking up the node tree until the culture is found. The
        /// <see cref="PublishedContentExtensions.GetCultureFromDomains"/> method in Umbraco only returns the name of
        /// the culture, which we can then look up using the static <see cref="CultureInfo.GetCultureInfo(string)"/>
        /// method. Using the static method ensures that we get a cached instance of <see cref="CultureInfo"/>, whereas
        /// using the <see cref="CultureInfo(string)"/> constructor instead would result in a new instance each time
        /// this method is called.</para>
        /// <para>Calling <see cref="PublishedContentExtensions.GetCultureFromDomains"/> on a content item that hasn't
        /// yet been published doesn't work properly, as the methods returns <c>null</c> instead of the actual culture
        /// node. This appears to be an issue with Umbraco as the internal logic fails to look up the route of the
        /// content item.
        ///
        /// While it ought to be fixed in Umbraco, this method has been updated to traverse up the tree until a culture
        /// has been found or the top of the tree has been reached. If none of the content items along the path specify
        /// a culture, the default culture configured in Umbraco (typically <c>en-US</c>) will be used as fallback.</para>
        /// </remarks>
        public static CultureInfo GetCultureInfo(this IPublishedContent content) {
            if (content == null) return CultureInfo.GetCultureInfo(global::Umbraco.Web.Composing.Current.UmbracoContext.Domains.DefaultCulture);
            string code = content.GetCultureFromDomains();
            return string.IsNullOrWhiteSpace(code) ? GetCultureInfo(content.Parent) : CultureInfo.GetCultureInfo(code);
        }

    }

}