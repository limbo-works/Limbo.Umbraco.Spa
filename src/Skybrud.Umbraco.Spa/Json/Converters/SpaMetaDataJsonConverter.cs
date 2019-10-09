using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Spa.Models.Meta;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Spa.Json.Converters {

    public class SpaMetaDataJsonConverter : JsonConverter {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            if (!(value is SpaMetaData)) throw new ArgumentException("Must be an instance of SpaMetaData", nameof(value));

            SpaMetaData data = (SpaMetaData) value;

            JObject obj = new JObject();
            JArray meta = new JArray();

            obj["title"] = data.MetaTitle ?? string.Empty;
            obj["meta"] = meta;

            AddMetaContent(meta, "description", data.MetaDescription, true);
            AddMetaContent(meta, "robots", data.Robots);

            if (data.OpenGraph != null) {

                AddMetaProperty(meta, "og:title", data.OpenGraph.Title);
                AddMetaProperty(meta, "og:description", data.OpenGraph.Description);
                AddMetaProperty(meta, "og:site_name", data.OpenGraph.SiteName);
                AddMetaProperty(meta, "og:url", data.OpenGraph.Url);

                foreach (SpaOpenGraphImage image in data.OpenGraph.Images) {
                    AddMetaProperty(meta, "og:image", image.Url);
                    if (image.Width > 0) AddMetaProperty(meta, "og:image:width", image.Width + string.Empty);
                    if (image.Height > 0) AddMetaProperty(meta, "og:image:height", image.Height + string.Empty);
                }

            }

            if (data.Links.Count > 0) obj.Add("link", JArray.FromObject(data.Links.Where(x => x.IsValid)));
            if (data.Scripts.Count > 0) obj.Add("script", JArray.FromObject(data.Scripts));

            obj.Add("__dangerouslyDisableSanitizers", new JArray(from str in data.DangerouslyDisableSanitizers select str));

            obj.WriteTo(writer);

        }

        protected void AddMetaContent(JArray meta, string name, string content, bool mandatory = false) {
            if (string.IsNullOrWhiteSpace(content) && mandatory == false) return;
            meta.Add(new JObject { { "name", name }, { "content", content ?? string.Empty } });
        }

        protected void AddMetaProperty(JArray meta, string property, string content, bool mandatory = false) {
            if (string.IsNullOrWhiteSpace(property) && mandatory == false) return;
            meta.Add(new JObject { { "property", property }, { "content", content ?? string.Empty } });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

    }

}