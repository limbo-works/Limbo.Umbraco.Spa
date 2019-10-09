// ReSharper disable InconsistentNaming

namespace Skybrud.Umbraco.Spa.Models.Meta.Twitter {
    
    /// <summary>
    /// Class representing a <strong>Summary with Large Image</strong> Twitter card.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.twitter.com/en/docs/tweets/optimize-with-cards/overview/summary-card-with-large-image</cref>
    /// </see>
    public class TwitterSummaryLargeImageCard : TwitterSummaryCard {

        /// <summary>
        /// Gets the type of the card.
        /// </summary>
        public override string Card => "summary_large_image";

    }

}