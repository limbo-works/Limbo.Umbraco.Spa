using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Spa.Models.Meta;
using Skybrud.Umbraco.Spa.Models.Meta.OpenGraph;

namespace Skybrud.Umbraco.Spa.Json.Converters {

    public class SpaMetaDataJsonConverter : JsonConverter {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            if (!(value is SpaMetaData)) throw new ArgumentException("Must be an instance of SpaMetaData", nameof(value));

            SpaMetaData data = (SpaMetaData) value;

            JObject obj = new JObject();
            JArray meta = new JArray();

            obj["title"] = data.MetaTitle ?? String.Empty;
            obj["meta"] = meta;

            AddMetaContent(meta, "description", data.MetaDescription, true);
            AddMetaContent(meta, "robots", data.Robots);
            AddMetaContent(meta, "og:title", data.OpenGraphTitle);
            AddMetaContent(meta, "og:description", data.OpenGraphDescription);
            AddMetaContent(meta, "og:site_name", data.OpenGraphSiteName);
            AddMetaContent(meta, "og:url", data.OpenGraphUrl);
            
            foreach (SpaOpenGraphImage image in data.OpenGraphImages) {
                AddMetaContent(meta, "og:image", image.Url);
                if (image.Width > 0) AddMetaContent(meta, "og:image:width", image.Width + "");
                if (image.Height > 0) AddMetaContent(meta, "og:image:height", image.Height + "");
            }

            if (data.Links.Count > 0) obj.Add("link", JArray.FromObject(data.Links.Where(x => x.IsValid)));
            if (data.Scripts.Count > 0) obj.Add("script", JArray.FromObject(data.Scripts));

            obj.Add("__dangerouslyDisableSanitizers", new JArray(from str in data.DangerouslyDisableSanitizers select str));

            obj.WriteTo(writer);

        }

        protected void AddMetaContent(JArray meta, string name, string content, bool mandatory = false) {
            if (String.IsNullOrWhiteSpace(content) && mandatory == false) return;
            meta.Add(new JObject { { "name", name }, { "content", content ?? String.Empty } });
        }

        protected void AddMetaProperty(JArray meta, string name, string property, bool mandatory = false) {
            if (String.IsNullOrWhiteSpace(property) && mandatory == false) return;
            meta.Add(new JObject { { "name", name }, { "property", property ?? String.Empty } });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

    }

}