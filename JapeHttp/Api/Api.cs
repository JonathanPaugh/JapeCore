namespace JapeHttp
{
    public class Api
    {
        private readonly string url;
        public string Key { get; }

        public Api(string url, string key = null)
        {
            this.url = url;
            Key = key;
        }

        public ApiRequest Request(string request)
        {
            return new ApiRequest(url + request);
        }

        public ApiRequest BaseRequest()
        {
            return new ApiRequest(url);
        }
    }
}
