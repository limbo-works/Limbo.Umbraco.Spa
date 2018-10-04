using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    public class SpaGridControl {

        [JsonProperty("value", Order = -99)]
        public object Value { get; }

        [JsonProperty("editor", Order = -98)]
        public SpaGridEditor Editor { get; }

        public SpaGridControl(object value, string editorAlias) {
            Editor = new SpaGridEditor(editorAlias);
            Value = value;
        }

        public SpaGridControl(object value, SpaGridEditor editor) {
            Editor = editor;
            Value = value;
        }

    }

}