using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing a SPA editor in the grid.
    /// </summary>
    public class SpaGridEditor {

        /// <summary>
        /// Gets the alias of the editor.
        /// </summary>
        [JsonProperty("alias", Order = -99)]
        public object Alias { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias">The alias of the editor.</param>
        public SpaGridEditor(string alias) {
            Alias = alias;
        }

    }

}