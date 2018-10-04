using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridModel {

        [JsonProperty("name", Order = -99)]
        public string Name { get; }

        [JsonProperty("sections", Order = -98)]
        public List<SpaGridSection> Sections { get; }

        public SpaGridModel(GridDataModel grid, IEnumerable<SpaGridSection> sections) {
            Name = grid.Name;
            Sections = sections.ToList();
        }

    }

}