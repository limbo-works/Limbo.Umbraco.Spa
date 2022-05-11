using System;
using Limbo.Umbraco.Spa.Models.Meta;
using Newtonsoft.Json;

#pragma warning disable 1591

namespace Limbo.Umbraco.Spa.Json.Converters {

    public class SpaMetaDataJsonConverter : JsonConverter {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (!(value is SpaMetaData meta)) throw new ArgumentException("Must be an instance of SpaMetaData", nameof(value));
            meta.ToVueMetaJson().WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

    }

}