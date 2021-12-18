using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace JapeHttp
{
    public static class IFileInfoExt
    {
        public static string Read(this IFileInfo file)
        {
            using StreamReader reader = new(file.CreateReadStream());
            return reader.ReadToEnd();
        }

        public static JsonData ReadJson(this IFileInfo file)
        {
            string json = Read(file);
            return new JsonData(json);
        }

        public static async Task<string> ReadAsync(this IFileInfo file)
        {
            using StreamReader reader = new(file.CreateReadStream());
            return await reader.ReadToEndAsync();
        }

        public static async Task<JsonData> ReadJsonAsync(this IFileInfo file)
        {
            string json = await ReadAsync(file);
            return new JsonData(json);
        }
    }
}
