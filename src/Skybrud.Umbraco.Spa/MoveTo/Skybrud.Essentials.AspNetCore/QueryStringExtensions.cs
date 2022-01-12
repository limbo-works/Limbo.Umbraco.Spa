using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Skybrud.Essentials.Strings;

namespace Skybrud.Umbraco.Spa.MoveTo.Skybrud.Essentials.AspNetCore {
    
    public static class QueryStringExtensions {
        
        /// <summary>
        /// Returns an URL encoded string representing the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query string to be encoded.</param>
        /// <returns>The URL encoded version of the query string.</returns>
        public static string ToUrlEncodedString(this IQueryCollection query) {

            StringBuilder sb = new StringBuilder();

            int i = 0;

            foreach (KeyValuePair<string, StringValues> pair in query) {

                foreach (string value in pair.Value) {
                    if (i++ > 0) sb.Append('&');
                    sb.Append(StringUtils.UrlEncode(pair.Key));
                    sb.Append('=');
                    sb.Append(StringUtils.UrlEncode(value));
                }

            }

            return sb.ToString();

        }

    }

}