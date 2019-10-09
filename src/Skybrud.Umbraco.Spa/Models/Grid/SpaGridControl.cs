using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing an area in the Umbraco grid.
    /// </summary>
    public class SpaGridControl {

        #region Properties

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        [JsonProperty("value", Order = -99)]
        public object Value { get; }

        /// <summary>
        /// Gets the editor of the control.
        /// </summary>
        [JsonProperty("editor", Order = -98)]
        public SpaGridEditor Editor { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="value"/> and <paramref name="editorAlias"/>.
        /// </summary>
        /// <param name="value">The value of the control.</param>
        /// <param name="editorAlias">The alias of the editor.</param>
        public SpaGridControl(object value, string editorAlias) {
            Editor = new SpaGridEditor(editorAlias);
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="value"/> and <paramref name="editor"/>.
        /// </summary>
        /// <param name="value">The value of the control.</param>
        /// <param name="editor">The editor of the control.</param>
        public SpaGridControl(object value, SpaGridEditor editor) {
            Editor = editor;
            Value = value;
        }

        #endregion

    }

}