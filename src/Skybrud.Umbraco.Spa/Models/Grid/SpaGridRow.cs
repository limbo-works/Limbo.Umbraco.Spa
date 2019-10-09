using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing a row in the grid.
    /// </summary>
    public class SpaGridRow {

        /// <summary>
        /// Gets the unique ID of the row.
        /// </summary>
        [JsonProperty("id", Order = -99)]
        public string Id { get; }

        /// <summary>
        /// Gets a dictionary representing the styles of the element.
        /// </summary>
        [JsonProperty("styles", Order = -98)]
        public object Styles { get; }

        /// <summary>
        /// Gets a dictionary representing the configuration (called Settings in the backoffice) of the element.
        /// </summary>
        [JsonProperty("config", Order = -97)]
        public object Config { get; }

        /// <summary>
        /// Gets a list of all areas added to this row.
        /// </summary>
        [JsonProperty("areas", Order = -96)]
        public List<SpaGridArea> Areas { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="row"/>.
        /// </summary>
        /// <param name="row">The row the instance should be based on.</param>
        public SpaGridRow(GridRow row) {
            Id = row.Id;
            Styles = row.Styles;
            Config = row.Config;
            Areas = new List<SpaGridArea>();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="row"/> and <paramref name="areas"/>.
        /// </summary>
        /// <param name="row">The row the instance should be based on.</param>
        /// <param name="areas">The areas that should be added to the row.</param>
        public SpaGridRow(GridRow row, IEnumerable<SpaGridArea> areas) {
            Id = row.Id;
            Styles = row.Styles;
            Config = row.Config;
            Areas = areas.ToList();
        }

    }

}