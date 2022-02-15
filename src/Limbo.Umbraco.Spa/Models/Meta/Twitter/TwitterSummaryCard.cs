using Newtonsoft.Json.Linq;

// ReSharper disable InconsistentNaming

namespace Limbo.Umbraco.Spa.Models.Meta.Twitter  {
    
    /// <summary>
    /// Class representing a <strong>Summary</strong> Twitter card.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.twitter.com/en/docs/tweets/optimize-with-cards/overview/summary</cref>
    /// </see>
    public class TwitterSummaryCard : ITwitterCard {

        /// <summary>
        /// Gets the type of the card.
        /// </summary>
        public virtual string Card => "summary";

        /// <summary>
        /// Gets or sets the username of the Twitter user representing the site.
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Gets or sets the username of the Twitter user the card should be attributed to.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets a title related to the content of the page.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a description that concisely summarizes the content as appropriate for presentation within a
        /// tweet. You should not re-use the title as the description or use this field to describe the general
        /// services provided by the website. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A URL to a unique image representing the content of the page. You should not use a generic image such as
        /// your website logo, author photo, or other image that spans multiple pages. Images for this Card support an
        /// aspect ratio of 2:1 with minimum dimensions of 300x157 or maximum of 4096x4096 pixels. Images must be less
        /// than 5MB in size. JPG, PNG, WEBP and GIF formats are supported. Only the first frame of an animated GIF
        /// will be used. SVG is not supported.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets a text description of the image conveying the essential nature of an image to users who are visually impaired. Maximum 420 characters.
        /// </summary>
        public string ImageText { get; set; }

        /// <summary>
        /// Writes the Twitter card data to the specified JSON <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to which the Twitter card data should be added.</param>
        public virtual void WriteJson(JArray array) {
            SpaUtils.Json.AddMetaContent(array, "twitter:card", Card);
            SpaUtils.Json.AddMetaContent(array, "twitter:site", Site);
            SpaUtils.Json.AddMetaContent(array, "twitter:creator", Creator);
            SpaUtils.Json.AddMetaContent(array, "twitter:title", Title);
            SpaUtils.Json.AddMetaContent(array, "twitter:description", Description);
            SpaUtils.Json.AddMetaContent(array, "twitter:image", Image);
            SpaUtils.Json.AddMetaContent(array, "twitter:image:alt", ImageText);
        }

    }

}