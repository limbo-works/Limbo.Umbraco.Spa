using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridRow {

        [JsonProperty("id", Order = -99)]
        public string Id { get; }

        [JsonProperty("styles", Order = -98)]
        public object Styles { get; }

        [JsonProperty("config", Order = -97)]
        public object Config { get; }

        [JsonProperty("areas", Order = -96)]
        public List<SpaGridArea> Areas { get; }

        public SpaGridRow(GridRow row) {
            Id = row.Id;
            Styles = row.Styles;
            Config = row.Config;
            Areas = new List<SpaGridArea>();
        }

        public SpaGridRow(GridRow row, IEnumerable<SpaGridArea> areas) {
            Id = row.Id;
            Styles = row.Styles;
            Config = row.Config;
            Areas = areas.ToList();
        }

    }

}