﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public static class HttpResponseExt
    {
        public static async Task Write(this HttpResponse response, string data)
        {
            await using StreamWriter writer = new(response.Body);
            await writer.WriteAsync(data);
        }

        public static async Task WriteJson(this HttpResponse response, JsonData data)
        {
            await Write(response, data.Serialize());
        }
    }
}