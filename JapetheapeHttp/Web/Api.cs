using System;
using System.IO;
using System.Net;
using System.Text;

namespace JapeHttp
{
    public class Api
    {
        private readonly string api;
        private readonly string key;

        public Api(string api, string key)
        {
            this.api = api;
            this.key = key;
        }

        public Request Request(string request)
        {
            return new Request(api + request);
        }
    }
}
