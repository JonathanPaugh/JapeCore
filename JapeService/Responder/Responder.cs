#nullable enable
#pragma warning disable 8600
#pragma warning disable 8604

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public interface IResponder
    {
        string Name { get; }

        Task<Request.Result> Invoke(ITransfer transfer, JsonData data, params object[] args);
    }

    public partial class Responder<T> : IEnumerable, IResponder
    {
        public string Name { get; }
        
        private readonly Indexer indexer;

        #pragma warning disable CS8714
        private readonly Dictionary<T, Executor> executors = new();
        #pragma warning restore CS8714

        private readonly List<RequestIntercepter> requestIntercepters = new();
        private readonly List<ResponseIntercepter> responseIntercepters = new();

        public delegate Task<Request.Result> Response(Transfer transfer, JsonData data, object[] args);

        public delegate Task<Request.Result> RequestInterception(Intercept intercept, object[] args);
        public delegate Task<Request.Result> ResponseInterception(Intercept intercept, JsonData data, object[] args);

        public delegate T Indexer(JsonData data);

        public Responder(string name, Indexer indexer)
        {
            Name = name;
            this.indexer = indexer;
        }

        public IEnumerator GetEnumerator() => executors.GetEnumerator();

        public T GetId(JsonData data) => indexer.Invoke(data);

        public void Add(T id, Response response) => executors.Add(id, new Executor(response));
        public void Remove(T id) => executors.Remove(id);

        public bool Contains(T id) => executors.ContainsKey(id);

        public void InterceptRequest(RequestInterception interception) => requestIntercepters.Add(new RequestIntercepter(interception));
        public void InterceptResponse(ResponseInterception interception) => responseIntercepters.Add(new ResponseIntercepter(interception));

        private bool Intercepted(Request.Result result)
        {
            if (result == null)
            {
                throw new ResultException($"{Name} Request: Null Resolution");
            }

            if (result is not Intercepter.Result interceptorResolution)
            {
                throw new ResultException($"{Name} Request: Invalid Resolution Type");
            }

            return interceptorResolution.Intercepted;
        }

        public async Task<Request.Result> Invoke(ITransfer transfer, JsonData data, params object[] args)
        {
            Request.Result result = null;

            try
            {
                result = await RespondPost(new Transfer(transfer.Request, transfer.Response, Execute), data, args);
            }
            catch (ResultException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                result = new Request.Result();
                Log.Write($"{Name} Request Error: Disposed");
            }
            catch (ResponderDataException)
            {
                result = new Request.Result();
                Log.Write($"{Name} Request Error: Invalid Data");
            }
            catch (Exception exception)
            {
                result = new Request.Result();
                Log.Write($"{Name} Request Error: Unknown (Information Logged)");
                Log.WriteLog(exception.ToString());
            }

            if (result == null)
            {
                throw new ResultException($"{Name} Request: Null Resolution");
            }

            return result;
        }

        public async Task Respond(HttpRequest request, HttpResponse response, params object[] args)
        {
            try
            {
                await RespondLow(request, response, args);
            }
            catch (ObjectDisposedException)
            {
                Log.Write($"{Name} Request Error: Disposed");
            }
            catch (Exception exception)
            {
                Log.Write($"{Name} Request Error: Unknown (Information Logged)");
                Log.WriteLog(exception.ToString());
            }
        }

        private async Task RespondLow(HttpRequest request, HttpResponse response, params object[] args)
        {
            Transfer transfer = new(request, response, Execute);

            Log.Write($"{Name} Request");

            foreach (RequestIntercepter intercepter in requestIntercepters)
            {
                Request.Result result = await intercepter.Invoke(new Intercept(request, response, Execute), args);
                if (Intercepted(result)) { return; }
            }

            JsonData data = await RespondData(transfer);

            foreach (ResponseIntercepter intercepter in responseIntercepters)
            {
                Request.Result result = await intercepter.Invoke(new Intercept(request, response, Execute), data, args);
                if (Intercepted(result)) { return; }
            }

            await RespondPost(transfer, data, args);
        }

        private async Task<JsonData> RespondData(Transfer transfer)
        {
            JsonData data;

            try
            {
                data = await GetData(transfer.request);
            }
            catch (ResponderDataException)
            {
                await transfer.Abort(Status.ErrorCode.BadRequest);
                throw;
            }

            return data;
        }

        private async Task<Request.Result> RespondPost(Transfer transfer, JsonData data, params object[] args)
        {
            object id;

            try
            {
                id = GetId(data);
            } 
            catch
            {
                Log.Write($"{Name} Request Error: Cannot Get Id");
                return await transfer.Abort(Status.ErrorCode.BadRequest);
            }

            if (id == null)
            {
                Log.Write($"{Name} Request Error: Null Id");
                return await transfer.Abort(Status.ErrorCode.BadRequest);
            }

            if (!Contains((T)id))
            {
                Log.Write($"{Name} Request Error: Invalid Id");
                return await transfer.Abort(Status.ErrorCode.NotFound);
            }

            return await Execute((T)id, transfer, data, args);
        }

        private async Task<JsonData> GetData(HttpRequest request)
        {
            JsonData data;

            try
            {
                data = await request.ReadJson();
            } 
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ResponderDataException(exception);
            }

            return data;
        }

        private async Task<Request.Result> Execute(T id, Transfer transfer, JsonData data, params object[] args)
        {
            Request.Result result = await executors[id].Invoke(transfer, data, args);
            if (result == null) { throw new ResultException($"{Name} Request: Null Resolution"); }
            return result;
        }
    }
}
