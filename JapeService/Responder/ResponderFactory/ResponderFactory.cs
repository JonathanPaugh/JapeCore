using System;
using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public partial class ResponderFactory
    {
        internal ResponderFactory() {}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public Builder<T> Create<T>(string name, Responder<T>.Indexer indexer)
        {
            return new Builder<T>(name, indexer);
        }
    }
}