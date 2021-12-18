using System.IO;
using System.Threading.Tasks;

namespace JapeCore
{
    public static class StreamExt
    {
        public static string Read(this Stream stream, bool keepAlive = false)
        {
            using StreamReader reader = new(stream, null, true, -1, keepAlive);
            return reader.ReadToEnd();
        }

        public static JsonData ReadJson(this Stream stream, bool keepAlive = false)
        {
            string json = Read(stream, keepAlive);
            return new JsonData(json);
        }

        public static async Task<string> ReadAsync(this Stream stream, bool keepAlive = false)
        {
            using StreamReader reader = new(stream, null, true, -1, keepAlive);
            return await reader.ReadToEndAsync();
        }

        public static async Task<JsonData> ReadJsonAsync(this Stream stream, bool keepAlive = false)
        {
            string json = await ReadAsync(stream, keepAlive);
            return new JsonData(json);
        }

        public static void Write(this Stream stream, string data, bool keepAlive = false)
        {
            using StreamWriter writer = new(stream, null, -1, keepAlive);
            writer.Write(data);
        }

        public static void WriteJson(this Stream stream, JsonData data, bool keepAlive = false)
        {
            Write(stream, data.Serialize(), keepAlive);
        }

        public static async Task WriteAsync(this Stream stream, string data, bool keepAlive = false)
        {
            await using StreamWriter writer = new(stream, null, -1, keepAlive);
            await writer.WriteAsync(data);
        }

        public static async Task WriteJsonAsync(this Stream stream, JsonData data, bool keepAlive = false)
        {
            await WriteAsync(stream, data.Serialize(), keepAlive);
        }

        public static void Rewind(this Stream stream) => stream.Position = 0;
    }
}
