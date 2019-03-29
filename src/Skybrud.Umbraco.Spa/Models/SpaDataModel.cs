using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Spa.Models {

    public class SpaDataModel {

        [JsonProperty("pageId", Order = -99)]
        public int PageId { get; set; }

        [JsonProperty("siteId", Order = -98)]
        public int SiteId { get; set; }

        [JsonProperty("contentGuid", Order = -97)]
        public Guid ContentGuid { get; set; }

        [JsonProperty("executeTimeMs", Order = 999)]
        public long ExecuteTimeMs { get; set; }

    }

}
