using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Spa.Models.Meta.OpenGraph {

    public class SpaOpenGraphProperties {

        #region Properties

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
        
        public SpaOpenGraphProperties(string baseUrl) {
            Images = new List<SpaOpenGraphImage>();
            BaseUrl = baseUrl;
        }

        public SpaOpenGraphProperties(IPublishedContent baseNode) {
            BaseUrl = String.Join("/", baseNode.UrlAbsolute().Split('/').Take(3));
            Images = new List<SpaOpenGraphImage>();
        }

        #endregion

        #region Member methods

        public void AppendImage(string image) {
            if (String.IsNullOrWhiteSpace(image)) return;
            image = image.StartsWith("/") ? BaseUrl + image : image;
            Images.Add(new SpaOpenGraphImage(image));
        }

        public void AppendImage(string image, int width, int height) {
            if (String.IsNullOrWhiteSpace(image)) return;
            image = image.StartsWith("/") ? BaseUrl + image : image;
            Images.Add(new SpaOpenGraphImage(image, width, height));
        }

        public void AppendImages(params string[] images) {

            if (images == null || images.Length == 0) return;

            List<SpaOpenGraphImage> temp = new List<SpaOpenGraphImage>();

            foreach (string imageUrl in images) {
                string url = imageUrl.StartsWith("/") ? BaseUrl + imageUrl : imageUrl;
                temp.Add(new SpaOpenGraphImage(url));
            }

            Images.AddRange(temp);

        }

        public void AppendImages(params IPublishedContent[] images) {

            if (images == null || images.Length == 0) return;
            
            List<SpaOpenGraphImage> temp = new List<SpaOpenGraphImage>();

            foreach (IPublishedContent image in images) {
                string url = BaseUrl + image.GetCropUrl(1200, 630);
                temp.Add(new SpaOpenGraphImage(url, 1200, 630));
            }

            Images.AddRange(temp);

        }

        public void PrependImages(PublishedContentModel content, params IPublishedContent[] images) {

            if (images == null || images.Length == 0) return;

            List<SpaOpenGraphImage> temp = new List<SpaOpenGraphImage>();

            foreach (IPublishedContent image in images) {
                string url = BaseUrl + image.GetCropUrl(1200, 630);
                temp.Add(new SpaOpenGraphImage(url, 1200, 630));
            }

            Images.InsertRange(0, temp);

        }

        #endregion

    }

}