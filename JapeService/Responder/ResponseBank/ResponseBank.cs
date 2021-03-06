using System.Collections;
using System.Collections.Generic;

namespace JapeService.Responder
{
    public class ResponseBank<T> : IEnumerable
    {
        private Dictionary<T, Responder<T>.Response> responses = new();

        public IEnumerator GetEnumerator() => responses.GetEnumerator();
        public void Add(T id, Responder<T>.Response response) => responses.Add(id, response);
        public void Remove(T id) => responses.Remove(id);
    }
}