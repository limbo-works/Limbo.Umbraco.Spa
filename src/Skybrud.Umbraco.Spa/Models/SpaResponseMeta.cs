using System.Net;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models {
    
    /// <summary>
    /// Class with meta data about the SPA response.
    /// </summary>
    public class SpaResponseMeta {

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        [JsonProperty("code")]
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Initializes a new instance based on <see cref="HttpStatusCode"/>.
        /// </summary>
        public SpaResponseMeta() {
            StatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public SpaResponseMeta(HttpStatusCode statusCode) {
            StatusCode = statusCode;
        }

    }

}