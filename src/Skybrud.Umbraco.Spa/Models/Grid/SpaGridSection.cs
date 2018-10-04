using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridSection {

        [JsonProperty("grid", Order = -99)]
        public int Grid { get; }

        [JsonProperty("rows", Order = -98)]
        public List<SpaGridRow> Rows { get; }

        public SpaGridSection(GridSection section) {
            Grid = section.Grid;
            Rows = new List<SpaGridRow>();
        }

        public SpaGridSection(GridSection section, IEnumerable<SpaGridRow> rows) {
            Grid = section.Grid;
            Rows = rows.ToList();
        }

    }

}