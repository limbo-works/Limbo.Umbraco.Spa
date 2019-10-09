using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing the overall grid mode.
    /// </summary>
    public class SpaGridModel {

        /// <summary>
        /// Gets the name of the layout of the grid.
        /// </summary>
        [JsonProperty("name", Order = -99)]
        public string Name { get; }

        /// <summary>
        /// Gets a list of all sections of the grid.
        /// </summary>
        [JsonProperty("sections", Order = -98)]
        public List<SpaGridSection> Sections { get; }

        /// <summary>
        /// Initializes a new instance from the specified <paramref name="grid"/>.
        /// </summary>
        /// <param name="grid">The grid model the new instance should be based on.</param>
        /// <param name="sections">The sections that should be added to the grid.</param>
        public SpaGridModel(GridDataModel grid, IEnumerable<SpaGridSection> sections) {
            Name = grid.Name;
            Sections = sections.ToList();
        }

    }

}