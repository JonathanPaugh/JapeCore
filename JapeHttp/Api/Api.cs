using System;
using JapeCore;

namespace JapeHttp
{
    public class Api
    {
        protected readonly string url;
        public string Key { get; }

        public Api(string url, string key = null)
        {
            this.url = url;
            Key = key;
        }

        public virtual ApiRequest Request(string path) => CreateRequest(url + path);

        protected static ApiRequest CreateRequest(string url) => new(url);
    }
}
