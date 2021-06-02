using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GetSanger.Services
{
    public static class ObjectJsonSerializer
    {
        class IgnoreJsonAttributesResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                foreach (var prop in props)
                {
                    prop.Ignored = false;   // Ignore [JsonIgnore]
                    prop.Converter = null;  // Ignore [JsonConverter]
                    prop.PropertyName = prop.UnderlyingName;  // restore original property name
                }
                return props;
            }
        }

        public static string SerializeForPage<T>(T i_Object)
        {
            return Uri.EscapeDataString(serializeHelper(i_Object, true));
        }

        public static string SerializeForServer<T>(T i_Object)
        {
            return serializeHelper(i_Object, false);
        }

        public static T DeserializeForServer<T>(string i_Json)
        {
            return deserializeHelper<T>(i_Json, false);
        }

        public static T DeserializeForPage<T>(string i_Json)
        {
            return deserializeHelper<T>(i_Json, true);
        }

        public static object DeserializeForAuth(string i_Json)
        {
            return ToObject(JToken.Parse(i_Json));
        }

        private static object ToObject(JToken i_Token)
        {
            switch (i_Token.Type)
            {
                case JTokenType.Object:
                    return i_Token.Children<JProperty>()
                        .ToDictionary(prop => prop.Name,
                            prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return i_Token.Select(ToObject).ToList();

                default:
                    return ((JValue)i_Token).Value;
            }
        }

        private static string serializeHelper<T>(T i_Object, bool i_IsForPage)
        {
            JsonSerializerSettings settings = null;
            if (i_IsForPage)
            {
                settings = new JsonSerializerSettings
                {
                    ContractResolver = new IgnoreJsonAttributesResolver(),
                    Formatting = Formatting.Indented
                };
            }

            return JsonConvert.SerializeObject(i_Object, settings);
        }

        private static T deserializeHelper<T>(string i_Object, bool i_IsForPage)
        {
            if (string.IsNullOrWhiteSpace(i_Object))
            {
                return default;
            }

            if (i_IsForPage)
            {
                i_Object = Uri.UnescapeDataString(i_Object);
            }

            return JsonConvert.DeserializeObject<T>(i_Object);
        }
    }
}
