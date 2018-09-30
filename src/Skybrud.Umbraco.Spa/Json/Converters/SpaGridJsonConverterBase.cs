using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Values;
using Umbraco.Web;
using Umbraco.Web.Templates;

namespace Skybrud.Umbraco.Spa.Json.Converters {

    public class SpaGridJsonConverterBase : JsonConverter {

        #region Properties

        public override bool CanRead => false;

        public override bool CanWrite => true;

        #endregion

        #region Member methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            GridDataModel grid = value as GridDataModel;
            if (grid == null) return;

            var hai = new {
                name = grid.Name,
                sections = from section in grid.Sections select new {
                    grid = section.Grid,
                    rows = from row in section.Rows select new {
                        id = row.Id,
                        name = row.Name,
                        styles = row.Styles.JObject,
                        config = row.Config.JObject,
                        areas = from area in row.Areas select new {
                            grid = area.Grid,
                            //allowAll = area.AllowAll,
                            //allowed = area.Allowed,
                            config = area.Config.JObject,
                            styles = area.Styles.JObject,
                            controls = from control in area.Controls select GetControl(control)
                        }
                    }
                }
            };

            JObject obj = JObject.FromObject(hai);

            obj.WriteTo(writer);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

        protected virtual object GetControl(GridControl control) {

            object value;

            switch (control.Editor.Alias) {

                case "rte":
                    value = GetRteParsedValue(control.GetValue<GridControlRichTextValue>());
                    break;

                default:
                    // Avoid returning too much data by default
                    return new JObject {
                        {"alias", control.Editor.Alias },
                        {"value", control.Value?.GetType().Name }
                    };

            }

            return new {
                value,
                editor = GetEditor(control.Editor)
            };

        }
        
        protected virtual object GetEditor(GridEditor editor) {
            // Other properties are ommitted since I'm not sure we need them for SPA solutions (including the config)
            return new {
                alias = editor.Alias,
            };
        }
        protected virtual string GetRteParsedValue(GridControlRichTextValue value) {
            return value == null ? null : TemplateUtilities.ParseInternalLinks(value.Value, UmbracoContext.Current.UrlProvider);
        }

        #endregion

    }

}