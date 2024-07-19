using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DotSpotifyWebWrapper.Utilities
{
    internal static class JsonUtilities
    {
        private static readonly JsonSerializerSettings s_settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static (T?, Exception?) DeserializeJsonString<T>(this string serializedString, bool convertSnakeCaseToPascalCase = false)
        {
            if (string.IsNullOrEmpty(serializedString))
                return (default, new ArgumentNullException(nameof(serializedString)));
            return (convertSnakeCaseToPascalCase) ? serializedString.DeserializeJsonString<T>(s_settings) :
                                                    serializedString.DeserializeJsonString<T>(new JsonSerializerSettings());
        }

        public static (T?, Exception?) DeserializeJsonString<T>(this string serializedString, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(serializedString))
                return (default, new ArgumentNullException(nameof(serializedString)));

            try
            {
                return (JsonConvert.DeserializeObject<T>(serializedString, settings), default);
            }
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException)
            {
                return (default, e);
            }
        }

        public static (string, Exception?) SerializeToJson<T>(this T obj, bool convertSnakeCaseToPascalCase = false)
        {
            return (convertSnakeCaseToPascalCase) ? obj.SerializeToJson(s_settings) :
                                                    obj.SerializeToJson(new JsonSerializerSettings());
        }

        public static (string, Exception?) SerializeToJson<T>(this T obj, JsonSerializerSettings settings)
        {
            try
            {
                return (JsonConvert.SerializeObject(obj, settings), default);
            }
            catch (Exception e) when (e is JsonWriterException || e is JsonSerializationException)
            {
                return (string.Empty, e);
            }
        }

        public static string TryGetContentForToken(this string jsonString, string token)
        {
            var obj = JObject.Parse(jsonString);
            var matchingToken = obj.SelectToken(token);
            if (matchingToken == default)
                return string.Empty;

            var s = matchingToken.ToString();
            return s;
        }
    }
}
