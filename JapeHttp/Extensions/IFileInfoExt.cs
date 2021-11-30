using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace JapeHttp
{
    public static class IFileInfoExt
    {
        public static async Task<string> Read(this IFileInfo file)
        {
            using StreamReader reader = new(file.CreateReadStream());
            return await reader.ReadToEndAsync();
        }

        public static async Task<JsonData> ReadJson(this IFileInfo file)
        {
            string json = await Read(file);
            return new JsonData(json);
        }
    }
}
