using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public abstract partial class Request
    {
        public const string NoCache = "no-cache";

        public enum Method { Get, Post, Create, Delete };

        internal readonly HttpRequest request;
        internal readonly HttpResponse response;

        protected Request(HttpRequest request, HttpResponse response)
        {
            this.request = request;
            this.response = response;
        }

        public Method GetMethod()
        {
            return request.Method switch
            {
                "GET" => Method.Get,
                "POST" => Method.Post,
                "CREATE" => Method.Create,
                "DELETE" => Method.Delete,
                _ => throw new ArgumentException(),
            };
        }

        protected async Task Close(int statusCode)
        {
            response.StatusCode = statusCode;
            await response.CompleteAsync();
        }

        protected async Task Close(int statusCode, string data)
        {
            response.StatusCode = statusCode;
            if (data != null) { await response.WriteAsync(data); }
            await response.CompleteAsync();
        }

        protected async Task Close(int statusCode, JsonData data)
        {
            response.StatusCode = statusCode;
            if (data != null) { await response.WriteJsonAsync(data); }
            await response.CompleteAsync();
        }

        protected static Result GenerateResult() => new();
    }
}
