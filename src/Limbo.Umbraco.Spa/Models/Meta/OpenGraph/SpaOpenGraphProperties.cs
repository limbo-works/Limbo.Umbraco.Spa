using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Spa.Models.Meta.OpenGraph {

    /// <summary>
    /// Class representing the Open Graph properties of a page.
    /// </summary>
    public class SpaOpenGraphProperties {

        #region Properties

        /// <summary>
        /// Gets the base URL of the current request. The base URL will automatically be prepended to images whose URL starts with a forward slash.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Gets the Open Graph title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the Open Graph description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the Open Graph site name.
        /// </summary>
        [JsonProperty("og:site_name")]
        public string SiteName { get; set; }

        /// <summary>
        /// Gets the Open Graph URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets a collection of Open Graph images.
        /// </summary>
        public List<SpaOpenGraphImage> Images { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance with the specified <paramref name="baseUrl"/>.
        /// </summary>
        /// <param name="baseUrl"></param>
        public SpaOpenGraphProperties(string baseUrl) {
            Images = new List<SpaOpenGraphImage>();
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Initializes a new instance using the specified <paramref name="rootNode"/> to determine the base URL of the current request.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        public SpaOpenGraphProperties(IPublishedContent rootNode) {
            BaseUrl = string.Join("/", rootNode.Url(mode: UrlMode.Absolute).Split('/').Take(3));
            Images = new List<SpaOpenGraphImage>();
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Appends an image with the specified <paramref name="url"/>.
        ///
        /// If <paramref name="url"/> starts with a forward slash, <see cref="BaseUrl"/> will automatically be
        /// prepended to the URL.
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        public void AppendImage(string url) {
            if (string.IsNullOrWhiteSpace(url)) return;
            url = url.StartsWith("/") ? BaseUrl + url : url;
            Images.Add(new SpaOpenGraphImage(url));
        }

        /// <summary>
        /// Appends an image with the specified <paramref name="url"/>, <paramref name="width"/> and <paramref name="height"/>.
        ///
        /// If <paramref name="url"/> starts with a forward slash, <see cref="BaseUrl"/> will automatically be
        /// prepended to the URL.
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public void AppendImage(string url, int width, int height) {
            if (string.IsNullOrWhiteSpace(url)) return;
            url = url.StartsWith("/") ? BaseUrl + url : url;
            Images.Add(new SpaOpenGraphImage(url, width, height));
        }

        /// <summary>
        /// Appends the images from the specified array of <paramref name="urls"/>.
        /// </summary>
        /// <param name="urls">The URLs of the images to append.</param>
        public void AppendImages(params string[] urls) {

            if (urls == null || urls.Length == 0) return;

            List<SpaOpenGraphImage> temp = new List<SpaOpenGraphImage>();

            foreach (string imageUrl in urls) {
                string url = imageUrl.StartsWith("/") ? BaseUrl + imageUrl : imageUrl;
                temp.Add(new SpaOpenGraphImage(url));
            }

            Images.AddRange(temp);

        }

        /// <summary>
        /// Appends the specified <paramref name="images"/>.
        /// </summary>
        /// <param name="images">An array of <see cref="IPublishedContent"/> representing the images to be appended.</param>
        public void AppendImages(params IPublishedContent[] images) {

            if (images == null || images.Length == 0) return;
            
            List<SpaOpenGraphImage> temp = new List<SpaOpenGraphImage>();

            foreach (IPublishedContent image in images) {
                string url = BaseUrl + image.GetCropUrl(1200, 630);
                temp.Add(new SpaOpenGraphImage(url, 1200, 630));
            }

            Images.AddRange(temp);

        }

        /// <summary>
        /// Writes the Open Graph data to the specified JSON <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to which the Open Graph data should be added.</param>
        public virtual void WriteJson(JArray array) {

            SpaUtils.Json.AddMetaProperty(array, "og:title", Title);
            SpaUtils.Json.AddMetaProperty(array, "og:description", Description);
            SpaUtils.Json.AddMetaProperty(array, "og:site_name", SiteName);
            SpaUtils.Json.AddMetaProperty(array, "og:url", Url);

            foreach (SpaOpenGraphImage image in Images) {
                SpaUtils.Json.AddMetaProperty(array, "og:image", image.Url);
                if (image.Width > 0) SpaUtils.Json.AddMetaProperty(array, "og:image:width", image.Width + string.Empty);
                if (image.Height > 0) SpaUtils.Json.AddMetaProperty(array, "og:image:height", image.Height + string.Empty);
            }

        }

        #endregion

    }

}