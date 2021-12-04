using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public abstract partial class Request
    {
        public enum Method { Get, Post, Create, Delete };

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

        protected internal readonly HttpRequest request;
        protected internal readonly HttpResponse response;

        protected Request(HttpRequest request, HttpResponse response)
        {
            this.request = request;
            this.response = response;
        }

        public abstract Task<Result> Complete(Status.SuccessCode code);
        public abstract Task<Result> Complete(Status.SuccessCode code, string data);
        public abstract Task<Result> Complete(Status.SuccessCode code, JsonData data);

        public abstract Task<Result> Abort(Status.ErrorCode code);

        protected async Task Close(int statusCode)
        {
            response.StatusCode = statusCode;
            await response.CompleteAsync();
        }

        protected async Task Close(int statusCode, string data)
        {
            response.StatusCode = statusCode;
            await response.Write(data);
            await response.CompleteAsync();
        }

        protected async Task Close(int statusCode, JsonData data)
        {
            response.StatusCode = statusCode;
            await response.WriteJson(data);
            await response.CompleteAsync();
        }

        protected static Result GenerateResult() => new();
    }
}
