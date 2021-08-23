using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace JapeHttp
{
    public static class Json
    {
        public static string ExtractObject(string data, string value)
        {
            return ((JsonElement)JsonSerializer.Deserialize<Dictionary<string, object>>(data)[value]).GetRawText();
        }
    }
}
