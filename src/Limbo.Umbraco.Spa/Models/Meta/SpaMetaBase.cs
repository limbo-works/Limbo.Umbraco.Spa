using Newtonsoft.Json;

namespace Limbo.Umbraco.Spa.Models.Meta {
    
    /// <summary>
    /// Represents a <c>base</c> HTML element.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.mozilla.org/en-US/docs/Web/HTML/Element/base</cref>
    /// </see>
    public class SpaMetaBase {

        #region Properties

        /// <summary>
        /// Gets or sets the base URL to be used throughout the document for relative URLs. Absolute and relative URLs
        /// are allowed.
        /// </summary>
        /// <see>
        ///     <cref>https://developer.mozilla.org/en-US/docs/Web/HTML/Element/base#attr-href</cref>
        /// </see>
        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets a keyword or author-defined name of the default browsing context to display the result when
        /// links or forms cause navigation, for <c>&lt;a&gt;</c> or <c>&lt;form&gt;</c> elements without an explicit
        /// target attribute. The attribute value targets a browsing context (such as a tab, window, or
        /// <c>&lt;iframe&gt;</c>).
        ///
        /// The following keywords have special meanings:
        /// <list type="bullet">
        ///     <item>
        ///         <c>_self</c>: Load the result into the same browsing context as the current one. (This is the default.)
        ///     </item>
        ///     <item>
        ///         <c>_blank</c>: Load the result into a new, unnamed browsing context.
        ///     </item>
        ///     <item>
        ///         <c>_parent</c>: Load the result into the parent browsing context of the current one. (If the
        /// current page is inside a frame.) If there is no parent, behaves the same way as <c>_self</c>.
        ///     </item>
        ///     <item>
        ///         <c>_top</c>: Load the result into the topmost browsing context (that is, the browsing context that
        /// is an ancestor of the current one, and has no parent). If there is no parent, behaves the same way as <c>_self</c>.
        ///     </item>
        /// </list>
        /// </summary>
        /// <see>
        ///     <cref>https://developer.mozilla.org/en-US/docs/Web/HTML/Element/base#attr-target</cref>
        /// </see>
        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <c>base</c> element with default options.
        /// </summary>
        public SpaMetaBase() { }

        /// <summary>
        /// Initializes a new <c>base</c> element based on the specified <paramref name="href"/>.
        /// </summary>
        /// <param name="href">The base URL to be used throughout the document for relative URLs. Absolute and relative URLs are allowed.</param>
        public SpaMetaBase(string href) {
            Href = href;
        }

        /// <summary>
        /// Initializes a new <c>base</c> element based on the specified <paramref name="href"/> and <paramref name="target"/>.
        /// </summary>
        /// <param name="href">The base URL to be used throughout the document for relative URLs. Absolute and relative URLs are allowed.</param>
        /// <param name="target">The value of the <c>target</c> attribute.</param>
        public SpaMetaBase(string href, string target) {
            Href = href;
            Target = target;
        }

        #endregion

    }

}