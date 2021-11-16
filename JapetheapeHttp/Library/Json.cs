using System.Collections.Generic;
using System.Text.Json;

namespace JapeHttp
{
    public static class Json
    {
        public static JsonElement DeserializeObject(object value)
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            Utf8JsonReader reader = new(bytes);
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }

        public static string Extract(string data, string key)
        {
            return ((JsonElement)JsonSerializer.Deserialize<Dictionary<string, object>>(data)[key]).GetRawText();
        }
    }
}
