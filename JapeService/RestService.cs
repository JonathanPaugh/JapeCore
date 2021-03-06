using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeService.Responder;

namespace JapeService
{
    public abstract class RestService : RequestService
    {
        private ResponderList responders = new();

        protected RestService(int http, int https) : base(http, https) {} 
        
        internal override async Task OnStartLow()
        {
            responders = Responders(new ResponderFactory());
            await base.OnStartLow();
        }

        protected Responder<T> GetResponder<T>(string name) => responders.Get<Responder<T>>(name);

        protected abstract ResponderList Responders(ResponderFactory factory);

        public class ResponderList : IEnumerable
        {
            private readonly Dictionary<string, IResponder> responders = new();

            public IEnumerator GetEnumerator() => responders.GetEnumerator();
            public void Add(IResponder responder) => responders.Add(responder.Name, responder);
            public void Remove(string key) => responders.Remove(key);
            internal T Get<T>(string key) where T : IResponder => (T)responders[key];
        }
    }
}
