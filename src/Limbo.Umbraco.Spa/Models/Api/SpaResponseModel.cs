using System.Net;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Api {
    
    /// <summary>
    /// Class representing the model of a SPA response.
    /// </summary>
    public class SpaResponseModel {

        #region Properties
        
        /// <summary>
        /// Gets or sets the meta data for the response.
        /// </summary>
        [JsonProperty(PropertyName = "meta")]
        public SpaMetaData Meta { get; set; }
        
        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        #endregion
        
        #region Constructors

        #endregion

        #region Static methods
        
        /// <summary>
        /// Creates a new error response with the specified error message.
        /// </summary>
        /// <param name="error">The error message of the response.</param>
        public static SpaResponseModel GetError(string error) {
            return GetError(HttpStatusCode.InternalServerError, error);
        }

        /// <summary>
        /// Creates a new error response with the specified status code and error message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="error">The error message of the response.</param>
        public static SpaResponseModel GetError(HttpStatusCode code, string error) {
            return GetError(code, error, null);
        }

        /// <summary>
        /// Creates a new error response with the specified status code and error message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="error">The error message of the response.</param>
        /// <param name="data">The data object.</param>
        public static SpaResponseModel GetError(HttpStatusCode code, string error, object data) {
            return new SpaResponseModel {
                Meta = { Code = code, Error = error },
                Data = data
            };
        }

        #endregion

    }

}