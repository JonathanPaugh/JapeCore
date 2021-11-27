namespace JapeHttp
{
    public class Api
    {
        private readonly string url;
        public string Key { get; }

        public Api(string url, string key)
        {
            this.url = url;
            Key = key;
        }

        public Request Request(string request)
        {
            return new Request(url + request);
        }
    }
}
