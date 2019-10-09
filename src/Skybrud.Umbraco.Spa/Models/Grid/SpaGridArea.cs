using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// Class representing an area in the Umbraco grid.
    /// </summary>
    public class SpaGridArea {

        /// <summary>
        /// Gets the column width of the area.
        /// </summary>
        [JsonProperty("grid", Order = -99)]
        public int Grid { get; }

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
        /// Gets a list of all controls added to this area.
        /// </summary>
        [JsonProperty("controls", Order = -96)]
        public List<SpaGridControl> Controls { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="area"/>.
        /// </summary>
        /// <param name="area">The area.</param>
        public SpaGridArea(GridArea area) {
            Grid = area.Grid;
            Styles = area.Styles;
            Config = area.Config;
            Controls = new List<SpaGridControl>();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="area"/> and <paramref name="controls"/>.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="controls">The controls.</param>
        public SpaGridArea(GridArea area, IEnumerable<SpaGridControl> controls) {
            Grid = area.Grid;
            Styles = area.Styles;
            Config = area.Config;
            Controls = controls.ToList();
        }

    }

}