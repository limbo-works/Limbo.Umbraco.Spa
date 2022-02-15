using Newtonsoft.Json.Linq;

// ReSharper disable once InconsistentNaming

namespace Limbo.Umbraco.Spa {

    /// <summary>
    /// Various utility methods for woroking with the SPA.
    /// </summary>
    public static class SpaUtils {

        /// <summary>
        /// Various utility methods for woroking with JSON.
        /// </summary>
        public static class Json {

            /// <summary>
            /// Adds a new <c>&lt;meta /&gt;</c> element with the specified <paramref name="name"/> and <paramref name="content"/> attributes.
            /// </summary>
            /// <param name="meta">The collection to which the <c>&lt;meta /&gt;</c> element will be appended.</param>
            /// <param name="name">The value of the <c>name</c> attribute.</param>
            /// <param name="content">The value of the <c>content</c> attribute.</param>
            /// <param name="mandatory">If <c>true</c> the <c>&lt;meta /&gt;</c> element will be appended regardless of <paramref name="content"/> being empty.</param>
            public static void AddMetaContent(JArray meta, string name, string content, bool mandatory = false) {
                if (string.IsNullOrWhiteSpace(content) && mandatory == false) return;
                meta.Add(new JObject { { "name", name }, { "content", content ?? string.Empty } });
            }

            /// <summary>
            /// Adds a new <c>&lt;meta /&gt;</c> element with the specified <paramref name="property"/> and <paramref name="content"/> attributes.
            /// </summary>
            /// <param name="meta">The collection to which the <c>&lt;meta /&gt;</c> element will be appended.</param>
            /// <param name="property">The value of the <c>property</c> attribute.</param>
            /// <param name="content">The value of the <c>content</c> attribute.</param>
            /// <param name="mandatory">If <c>true</c> the <c>&lt;meta /&gt;</c> element will be appended regardless of <paramref name="content"/> being empty.</param>
            public static void AddMetaProperty(JArray meta, string property, string content, bool mandatory = false) {
                if (string.IsNullOrWhiteSpace(property) && mandatory == false) return;
                meta.Add(new JObject { { "property", property }, { "content", content ?? string.Empty } });
            }

        }

    }

}