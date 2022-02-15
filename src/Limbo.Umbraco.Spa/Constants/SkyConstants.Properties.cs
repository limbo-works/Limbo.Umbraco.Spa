using UmbracoContentConstants = Umbraco.Cms.Core.Constants.Conventions.Content;

// ReSharper disable InconsistentNaming

namespace Limbo.Umbraco.Spa.Constants {

    public static partial class SkyConstants {

        /// <summary>
        /// Constants for common property aliases.
        /// </summary>
        public static class Properties {

            /// <summary>
            /// Gets the default date property alias.
            /// </summary>
            public const string ContentDate = "contentDate";

            /// <summary>
            /// Gets the default grid property alias.
            /// </summary>
            public const string Grid = "grid";

            /// <summary>
            /// Gets the default hide from internal search + no-follow meta.
            /// </summary>
            public const string HideFromSearch = "hideFromSearch";

            /// <summary>
            /// Gets the default JSON debug property alias.
            /// </summary>
            public const string JsonData = "jsonData";

            /// <summary>
            /// Gets the default property alias for a "no cache" option.
            /// </summary>
            public const string NoCache = "noCache";

            /// <summary>
            /// Gets the default property alias for the <strong>Open Graph</strong> title.
            /// </summary>
            public const string OpenGraphTitle = "ogTitle";

            /// <summary>
            /// Gets the default property alias for the <strong>Open Graph</strong> description.
            /// </summary>
            public const string OpenGraphDescription = "ogDescription";

            /// <summary>
            /// Gets the default property alias for the <strong>Open Graph</strong> image.
            /// </summary>
            public const string OpenGraphImage = "ogImage";

            /// <summary>
            /// Gets the default property alias for the SEO title.
            /// </summary>
            public const string SeoTitle = "seoTitle";

            /// <summary>
            /// Gets the default property alias for the SEO meta description.
            /// </summary>
            public const string SeoMetaDescription = "seoMetaDescription";

            /// <summary>
            /// Gets the default property alias for the site name.
            /// </summary>
            public const string SiteName = "siteName";

            /// <summary>
            /// Gets the default property alias for the 404 page.
            /// </summary>
            public const string NotFoundPage = "skyNotFoundPage";

            /// <summary>
            /// Gets the default property alias for the teaser text.
            /// </summary>
            public const string Teaser = "teaser";

            /// <summary>
            /// Gets the default property alias for the page title.
            /// </summary>
            public const string Title = "title";

            /// <summary>
            /// Same as <see cref="UmbracoContentConstants.InternalRedirectId"/>.
            /// </summary>
            public const string UmbracoInternalRedirect = UmbracoContentConstants.InternalRedirectId;

            /// <summary>
            /// Same as <see cref="UmbracoContentConstants.NaviHide"/>.
            /// </summary>
            public const string UmbracoNaviHide = UmbracoContentConstants.NaviHide;

            /// <summary>
            /// Gets the default property alias for the main navigation.
            /// </summary>
            public const string PrimaryNavigation = "mainNavigation";

            /// <summary>
            /// Gets the default property alias for the secondary navigation.
            /// </summary>
            public const string SecondaryNavigation = "secondaryNavigation";

        }


    }

}