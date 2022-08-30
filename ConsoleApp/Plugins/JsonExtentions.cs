using ConsoleApp.Plugins;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace System
{
    public static class JsonExtentions
    {
        //如果不设置这个，那么"雅思真题"就会保存为"\u96C5\u601D\u771F\u9898"
        public readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        public static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = false)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                Encoder = Encoder,
                PropertyNameCaseInsensitive = true
            };
            if (camelCase)
            {
                options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.PropertyNameCaseInsensitive = false;
            }
            options.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            return options;
        }

        public static string ToJsonString(this object value, bool camelCase = false)
        {
            var option = CreateJsonSerializerOptions(camelCase);
            return JsonSerializer.Serialize(value, value.GetType(), option);
        }

        public static T ParseJson<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            var option = CreateJsonSerializerOptions();

            return JsonSerializer.Deserialize<T>(value, option);
        }
    }
}
