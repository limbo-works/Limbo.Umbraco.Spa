using Newtonsoft.Json;

namespace Limbo.Umbraco.Spa.Models.Meta.OpenGraph {

    /// <summary>
    /// Class with information about an Open Graph image.
    /// </summary>
    public class SpaOpenGraphImage {

        /// <summary>
        /// Gets or sets the URL of the image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }
        
        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        public SpaOpenGraphImage(string url) {
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="url"/>, <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public SpaOpenGraphImage(string url, int width, int height) {
            Url = url;
            Width = width;
            Height = height;
        }

    }

}