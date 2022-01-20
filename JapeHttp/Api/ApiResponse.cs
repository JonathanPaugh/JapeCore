using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using JapeCore;

namespace JapeHttp
{
    public class ApiResponse : IDisposable
    {
        private readonly WebResponse response;
        private bool disposed;

        private ApiResponse(WebResponse response) { this.response = response; } 

        public string Read() => response.GetResponseStream().Read(true);
        public async Task<string> ReadAsync() => await response.GetResponseStream().ReadAsync(true);
        public JsonData ReadJson() => response.GetResponseStream().ReadJson(true);
        public async Task<JsonData> ReadJsonAsync() => await response.GetResponseStream().ReadJsonAsync(true);

        internal static ApiResponse FromRequest(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException exception)
            {
                ThrowResponseException(exception);
                throw;
            }

            return new ApiResponse(response);
        }

        internal static async Task<ApiResponse> FromRequestAsync(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch (WebException exception)
            {
                ThrowResponseException(exception);
                throw;
            }

            return new ApiResponse(response);
        }

        private static void ThrowResponseException(WebException exception)
        {
            if (exception.Status != WebExceptionStatus.ProtocolError) { return; }

            HttpWebResponse response = (HttpWebResponse)exception.Response;
            Status.ErrorCode statusCode = (Status.ErrorCode)response.StatusCode;
            response.Dispose();
            throw new ResponseException(statusCode, exception);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    response.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
