using Newtonsoft.Json.Linq;

namespace Skybrud.Umbraco.Spa.Models.Meta.Twitter  {

    /// <summary>
    /// Interface representing a Twitter card.
    /// </summary>
    public interface ITwitterCard {

        /// <summary>
        /// Gets the type of the card - eg. <c>summary_large_image</c>.
        /// </summary>
        string Card { get; }

        /// <summary>
        /// Gets or sets the username of the Twitter user representing the site.
        /// </summary>
        string Site { get; }

        /// <summary>
        /// Gets or sets the username of the Twitter user the card should be attributed to.
        /// </summary>
        string Creator { get; }

        /// <summary>
        /// Writes the Twitter card data to the specified JSON <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to which the Twitter card data should be added.</param>
        void WriteJson(JArray array);

    }

}