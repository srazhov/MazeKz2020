using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebMaze.Services
{
    public static class DeserializeExtensions
    {
        private static readonly JsonSerializerOptions CaseInsensitiveSerializerOptions =
            new JsonSerializerOptions() {PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };
        
        public static T Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T DeserializeCustom<T>(this string json, JsonSerializerOptions customSerializerOptions)
        {
            return JsonSerializer.Deserialize<T>(json, customSerializerOptions);
        }

        public static T DeserializeCaseInsensitive<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, CaseInsensitiveSerializerOptions);
        }
    }
}
