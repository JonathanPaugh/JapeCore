using System.Collections.Generic;

namespace JapeService.Responder
{
    public partial class ResponderFactory
    {
        public class Builder<T>
        {
            private readonly Responder<T> responder;

            internal Builder(string name, Responder<T>.Indexer indexer)
            {
                responder = new Responder<T>(name, indexer);
            }

            public Builder<T> Responses(ResponseBank<T> responses)
            {
                foreach (KeyValuePair<T, Responder<T>.Response> response in responses)
                {
                    responder.Add(response.Key, response.Value);
                }
                return this;
            }

            public Builder<T> InterceptRequest(Responder<T>.RequestInterception interception)
            {
                responder.InterceptRequest(interception);
                return this;
            }

            public Builder<T> InterceptResponse(Responder<T>.ResponseInterception interception)
            {
                responder.InterceptResponse(interception);
                return this;
            }

            public IResponder Build()
            {
                return responder;
            }
        }
    }
}