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

        Task<Resolution> Invoke(ITransfer transfer, JsonData data, params object[] args);
    }

    public partial class Responder<T> : IEnumerable, IResponder
    {
        public string Name { get; }
        
        private readonly Func<JsonData, T> indexer;

        #pragma warning disable CS8714
        private readonly Dictionary<T, Executor> executors = new();
        #pragma warning restore CS8714

        private readonly List<RequestIntercepter> requestIntercepters = new();
        private readonly List<ResponseIntercepter> responseIntercepters = new();

        public Responder(string name, Func<JsonData, T> indexer)
        {
            Name = name;
            this.indexer = indexer;
        }

        public IEnumerator GetEnumerator() => executors.GetEnumerator();

        public T GetId(JsonData data) => indexer.Invoke(data);

        public void Add(T id, Func<Transfer, JsonData, object[], Task<Resolution>> response) => executors.Add(id, new Executor(response));
        public void Remove(T id) => executors.Remove(id);

        public bool Contains(T id) => executors.ContainsKey(id);

        public void InterceptRequest(Func<Intercept, object[], Task<Resolution>> onIntercept) => requestIntercepters.Add(new RequestIntercepter(onIntercept));
        public void InterceptResponse(Func<Intercept, JsonData, object[], Task<Resolution>> onIntercept) => responseIntercepters.Add(new ResponseIntercepter(onIntercept));

        private bool Intercepted(Resolution resolution)
        {
            if (resolution == null)
            {
                Log.Write($"{Name} Request Warning: Null Resolution");
                return false;
            }

            if (resolution is not Intercepter.Resolution interceptorResolution)
            {
                Log.Write($"{Name} Request Warning: Resolution Wrong Type");
                return false;
            }

            return interceptorResolution.Intercepted;
        }

        public async Task<Resolution> Invoke(ITransfer transfer, JsonData data, params object[] args)
        {
            Resolution resolution = null;

            try
            {
                resolution = await RespondPost(new Transfer(transfer.Request, transfer.Response, Execute), data, args);
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

            return resolution;
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

        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        private async Task RespondLow(HttpRequest request, HttpResponse response, params object[] args)
        {
            Transfer transfer = new(request, response, Execute);

            Log.Write($"{Name} Request");

            foreach (RequestIntercepter intercepter in requestIntercepters)
            {
                Resolution resolution = await intercepter.Invoke(new Intercept(request, response, Execute), args);
                if (Intercepted(resolution)) { return; }
            }

            JsonData data = await RespondData(transfer);

            if (data == null) { return; }

            foreach (ResponseIntercepter intercepter in responseIntercepters)
            {
                Resolution resolution = await intercepter.Invoke(new Intercept(request, response, Execute), data, args);
                if (Intercepted(resolution)) { return; }
            }

            await RespondPost(transfer, data, args);
        }

        private async Task<JsonData> RespondData(Transfer transfer)
        {
            JsonData data = await GetData(transfer.request);

            if (data == null)
            {
                Log.Write($"{Name} Request Error: Empty Data");
                await transfer.Abort(Status.ErrorCode.BadRequest);
                return data;
            }

            return data;
        }

        private async Task<Resolution> RespondPost(Transfer transfer, JsonData data, params object[] args)
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
            JsonData data = null;

            try
            {
                data = await request.ReadJson();
            } 
            catch (ObjectDisposedException) { throw; }
            catch (Exception)
            {
                Log.Write($"{Name} Request Error: Cannot Read Data");
                return data;
            }

            return data;
        }

        private async Task<Resolution> Execute(T id, Transfer transfer, JsonData data, params object[] args)
        {
            Resolution resolution = await executors[id].Invoke(transfer, data, args);
            if (resolution == null)
            {
                Log.Write($"{Name} Request Warning: Null Resolution");
            }
            return resolution;
        }
    }
}
