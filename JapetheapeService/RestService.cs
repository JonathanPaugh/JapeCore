using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using JapeService.Responder;

namespace JapeService
{
    public abstract class RestService : Service
    {
        private ResponderList responders = new();

        protected RestService(int http, int https) : base(http, https) {} 
        
        internal override void OnStartLow()
        {
            responders = Responders(new ResponderFactory());
            base.OnStartLow();
        }

        protected Responder<T> GetResponder<T>(string name) => responders.Get<Responder<T>>(name);

        protected abstract ResponderList Responders(ResponderFactory factory);

        public class ResponderList : IEnumerable
        {
            private Dictionary<string, IResponder> responders = new();

            public IEnumerator GetEnumerator() => responders.GetEnumerator();
            public void Add(IResponder responder) => responders.Add(responder.Name, responder);
            public void Remove(string key) => responders.Remove(key);
            internal T Get<T>(string key) where T : IResponder => (T)responders[key];
        }
    }
}
