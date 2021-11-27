using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public class ResponderFactory
    {
        internal ResponderFactory() {}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public Builder<T> Create<T>(string name, Func<JsonData, T> indexer)
        {
            return new Builder<T>(name, indexer);
        }

        public class Builder<T>
        {
            private readonly Responder<T> responder;

            internal Builder(string name, Func<JsonData, T> indexer)
            {
                responder = new Responder<T>(name, indexer);
            }

            public Builder<T> Responses(ResponseBank<T> responses)
            {
                foreach (KeyValuePair<T, Func<Responder<T>.Transfer, JsonData, object[], Task<Resolution>>> response in responses)
                {
                    responder.Add(response.Key, response.Value);
                }
                return this;
            }

            public Builder<T> InterceptRequest(Func<Responder<T>.Intercept, object[], Task<Resolution>> onIntercept)
            {
                responder.InterceptRequest(onIntercept);
                return this;
            }

            public Builder<T> InterceptResponse(Func<Responder<T>.Intercept, JsonData, object[], Task<Resolution>> onIntercept)
            {
                responder.InterceptResponse(onIntercept);
                return this;
            }

            public IResponder Build()
            {
                return responder;
            }
        }
    }
}