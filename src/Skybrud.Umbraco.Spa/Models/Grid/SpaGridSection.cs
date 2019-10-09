using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing a section in the grid.
    /// </summary>
    public class SpaGridSection {

        /// <summary>
        /// Gets the overall column width of the section.
        /// </summary>
        [JsonProperty("grid", Order = -99)]
        public int Grid { get; }

        /// <summary>
        /// Gets a list of all rows in the sections.
        /// </summary>
        [JsonProperty("rows", Order = -98)]
        public List<SpaGridRow> Rows { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section the new instance should be based on.</param>
        public SpaGridSection(GridSection section) {
            Grid = section.Grid;
            Rows = new List<SpaGridRow>();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section the new instance should be based on.</param>
        /// <param name="rows">The rows that should be added to the section.</param>
        public SpaGridSection(GridSection section, IEnumerable<SpaGridRow> rows) {
            Grid = section.Grid;
            Rows = rows.ToList();
        }

    }

}