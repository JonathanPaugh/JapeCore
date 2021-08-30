using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace JapeHttp
{
    public class Listener
    {
        private HttpListener listener;
        private Action<HttpListenerRequest, HttpListenerResponse> onRequest;

        public Listener(int port, Action<HttpListenerRequest, HttpListenerResponse> onRequest)
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");
            this.onRequest = onRequest;
        }

        public void Start()
        {
            listener.Start();
            listener.BeginGetContext(Process, listener);
        }

        private void Process(IAsyncResult result)
        {
            HttpListenerContext context = ((HttpListener)result.AsyncState).EndGetContext(result);
            listener.BeginGetContext(Process, listener);

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            onRequest(request, response);
        }
    }
}
