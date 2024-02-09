using System;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skybrud.Essentials.Json.Newtonsoft.Converters;
using Skybrud.Essentials.Strings;

#pragma warning disable 1591

namespace Limbo.Umbraco.Spa.Json.Resolvers {

    public class SpaPublishedContentContractResolver : DefaultContractResolver {

        #region Member methods

        protected virtual bool ShouldSerialize(MemberInfo member, JsonProperty property) {

            // Ignored unwanted properties from types in the Umbraco.Core.Models.PublishedContent namespace
            if (member.DeclaringType?.Namespace == "Umbraco.Cms.Core.Models.PublishedContent") {
                switch (member.Name) {
                    case "CompositionAliases":
                    case "ContentSet":
                    case "ContentType":
                    case "PropertyTypes":
                    case "Properties":
                    case "Parent":
                    case "Children":
                    case "DocumentTypeId":
                    case "WriterName":
                    case "CreatorName":
                    case "Cultures":
                    case "ChildrenForAllCultures":
                    case "UrlSegment":
                    case "WriterId":
                    case "CreatorId":
                    case "CreateDate":
                    case "UpdateDate":
                    case "Version":
                    case "SortOrder":
                    case "TemplateId":
                    case "IsDraft":
                    case "ItemType":
                        return false;
                }
            }

            // Ignore other unwanted properties
            switch (member.Name) {
                case "SeoMetaDescription":
                case "Seodashboard":
                case "Preview":
                case "SeoTitle":
                    return false;
            }

            if (member is PropertyInfo pi) {
                switch (pi.PropertyType.FullName) {
                    case "Skybrud.Separator.SeparatorModel":
                        return false;
                }
            }

            return true;

        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {

            // Get a JsonProperty instance from the parent
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Should we serialize the property?
			property.ShouldSerialize = _ => ShouldSerialize(member, property);

            // Make sure the property names are in lower camel case
            property.PropertyName = StringUtils.ToCamelCase(property.PropertyName);

            // Overwrite the order of certain properties
            property.Order = member.Name switch {
                "Id" => -99,
                "Key" => -98,
                "Name" => -97,
                "Level" => -96,
                "Url" => -95,
                _ => property.Order
            };

            return property;
        }

        protected override JsonContract CreateContract(Type objectType) {

            JsonContract contract = base.CreateContract(objectType);

            // this will only be called once and then cached
            if (objectType == typeof(HtmlString)) {
                contract.Converter = new StringJsonConverter();
            }

            return contract;

        }

        #endregion

    }

}