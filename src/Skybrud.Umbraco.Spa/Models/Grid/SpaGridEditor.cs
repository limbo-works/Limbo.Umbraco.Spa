using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridEditor {

        [JsonProperty("alias", Order = -99)]
        public object Alias { get; }

        public SpaGridEditor(string alias) {
            Alias = alias;
        }

    }

}