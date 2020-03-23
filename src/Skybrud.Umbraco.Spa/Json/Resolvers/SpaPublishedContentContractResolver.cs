using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skybrud.Essentials.Strings;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.Spa.Json.Converters;
using System.Web;
using Skybrud.Essentials.Json.Converters;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Spa.Json.Resolvers {

    public class SpaPublishedContentContractResolver : DefaultContractResolver {

        #region Properties

        public SpaGridJsonConverterBase GridConverter { get; }

        #endregion

        #region Constructors

        public SpaPublishedContentContractResolver() {
            GridConverter = new SpaGridJsonConverterBase();
        }

        public SpaPublishedContentContractResolver(SpaGridJsonConverterBase gridConverter) {
            GridConverter = gridConverter;
        }

        #endregion

        #region Member methods

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {

            JsonProperty property = base.CreateProperty(member, memberSerialization);
            
			property.ShouldSerialize = instance => {

                if (property.PropertyName == "CompositionAliases") return false;
                if (property.PropertyName == "ContentSet") return false;
                if (property.PropertyName == "PropertyTypes") return false;
                if (property.PropertyName == "Properties") return false;
                if (property.PropertyName == "Parent") return false;
                if (property.PropertyName == "Children") return false;
                if (property.PropertyName == "DocumentTypeId") return false;
                if (property.PropertyName == "WriterName") return false;
                if (property.PropertyName == "CreatorName") return false;
                if (property.PropertyName == "WriterId") return false;
                if (property.PropertyName == "CreatorId") return false;
                if (property.PropertyName == "CreateDate") return false;
                if (property.PropertyName == "UpdateDate") return false;
                if (property.PropertyName == "Version") return false;
                if (property.PropertyName == "SortOrder") return false;
                if (property.PropertyName == "TemplateId") return false;
                if (property.PropertyName == "IsDraft") return false;
                if (property.PropertyName == "ItemType") return false;
                if (property.PropertyName == "ContentType") return false;
                if (property.PropertyName == "ContentSet") return false;
                if (property.PropertyName == "Path") return false; //override path with patharray to make it an array
                if (property.PropertyName == "SeoMetaDescription") return false;
                if (property.PropertyName == "Seodashboard") return false;
                if (property.PropertyName == "Preview") return false;
		        if (property.PropertyName == "SeoTitle") return false;
		        if (property.PropertyName == "UrlSegment") return false;
		        if (property.PropertyName == "Cultures") return false;
		        if (property.PropertyName == "ChildrenForAllCultures") return false;
                //ADD CUSTOM OVERRRIDES AFTER THIS IN THE ABOVE FORMAT


                property.PropertyName = StringUtils.ToCamelCase(property.PropertyName);

                return true;
            };

            switch (property.PropertyName) {

                case "Id":
                    property.Order = -99;
                    break;

                case "Key":
                    property.Order = -98;
                    break;

                case "Name":
                    property.Order = -97;
                    break;

                case "Level":
                    property.Order = -96;
                    break;

                case "Url":
                    property.Order = -95;
                    break;

            }


            return property;
        }

        protected override JsonContract CreateContract(Type objectType) {

            JsonContract contract = base.CreateContract(objectType);

            // this will only be called once and then cached
            if (objectType == typeof(GridDataModel)) {
                contract.Converter = GridConverter;
            } else if (objectType == typeof(HtmlString) || objectType == typeof(IHtmlString)) {
                contract.Converter = new ToStringJsonConverter();
            }		

            return contract;

        }

        #endregion

    }

}