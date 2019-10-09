using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Values;
using Skybrud.Umbraco.Spa.Models.Grid;
using Umbraco.Web.Composing;
using Umbraco.Web.Templates;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Spa.Json.Converters {

    public class SpaGridJsonConverterBase : JsonConverter {

        #region Properties

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public bool SkipInvalidControls { get; set; }

        #endregion

        #region Constructors

        public SpaGridJsonConverterBase() {
            SkipInvalidControls = true;
        }

        #endregion

        #region Member methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            if (!(value is GridDataModel grid)) return;

            List<SpaGridSection> sections = new List<SpaGridSection>();

            foreach (GridSection section in grid.Sections) {
                
                List<SpaGridRow> rows = new List<SpaGridRow>();

                foreach (GridRow row in section.Rows) {

                    List<SpaGridArea> areas = new List<SpaGridArea>();

                    foreach (GridArea area in row.Areas) {

                        List<SpaGridControl> controls = new List<SpaGridControl>();

                        foreach (GridControl control in area.Controls) {
                            SpaGridControl c = GetControl(control);
                            if (SkipInvalidControls && c == null) continue;
                            controls.Add(c);
                        }

                        if (controls.Count > 0) areas.Add(new SpaGridArea(area, controls));

                    }

                    if (areas.Count > 0) rows.Add(new SpaGridRow(row, areas));

                }

                if (rows.Count > 0) sections.Add(new SpaGridSection(section, rows));

            }

            SpaGridModel hai = new SpaGridModel(grid, sections);

            JObject obj = JObject.FromObject(hai);

            obj.WriteTo(writer);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

        protected virtual SpaGridControl GetControl(GridControl control) {

            object value;
            SpaGridEditor editor = GetEditor(control.Editor);

            switch (control.Editor.Alias) {

                case "rte":
                    value = GetRteParsedValue(control.GetValue<GridControlRichTextValue>());
                    break;

                default:
                    // Avoid returning too much data by default
                    return new SpaGridControl(control.Value?.GetType().Name, control.Editor.Alias);

            }

            return new SpaGridControl(value, editor);

        }
        
        protected virtual SpaGridEditor GetEditor(GridEditor editor) {
            // Other properties are ommitted since I'm not sure we need them for SPA solutions (including the config)
            return new SpaGridEditor(editor.Alias);
        }
        protected virtual string GetRteParsedValue(GridControlRichTextValue value) {
            return value == null ? null : TemplateUtilities.ParseInternalLinks(value.Value, Current.UmbracoContext.UrlProvider);
        }

        #endregion

    }

}