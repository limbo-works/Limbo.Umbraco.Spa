using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Meta.OpenGraph {

    public class SpaOpenGraphImage {

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        public SpaOpenGraphImage(string url, int width, int height) {
            Url = url;
            Width = width;
            Height = height;
        }

    }

}