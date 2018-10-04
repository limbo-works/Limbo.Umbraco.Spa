using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridArea {

        [JsonProperty("grid", Order = -99)]
        public int Grid { get; }

        [JsonProperty("styles", Order = -98)]
        public object Styles { get; }

        [JsonProperty("config", Order = -97)]
        public object Config { get; }

        [JsonProperty("controls", Order = -96)]
        public List<SpaGridControl> Controls { get; }

        public SpaGridArea(GridArea area) {
            Grid = area.Grid;
            Styles = area.Styles;
            Config = area.Config;
            Controls = new List<SpaGridControl>();
        }

        public SpaGridArea(GridArea area, IEnumerable<SpaGridControl> controls) {
            Grid = area.Grid;
            Styles = area.Styles;
            Config = area.Config;
            Controls = controls.ToList();
        }

    }

}