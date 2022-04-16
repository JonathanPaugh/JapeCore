using System;
using System.Text;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JapeWeb
{
    public class Mapping : WebComponent
    {
        public enum Method { Get, Post, Any }
        public enum ReadType { Binary, File, FileAsync }

        private readonly Method method;
        private readonly ReadType readType;

        private readonly PathString requestPath;
        private readonly string responsePath;

        private readonly Encoding encoding;

        private readonly ReadBinary readBinary;
        private readonly ReadFile readFile;
        private readonly ReadFileAsync readFileAsync;
    
        public delegate byte[] ReadBinary(string path, Encoding encoding);
        public delegate string ReadFile(string path);
        public delegate Task<string> ReadFileAsync(string path);

        private Mapping(Method method,
                        ReadType readType,
                        PathString requestPath, 
                        string responsePath, 
                        Encoding encoding, 
                        ReadBinary readBinary, 
                        ReadFile readFile, 
                        ReadFileAsync readFileAsync)
        {
            this.method = method;
            this.readType = readType;
            this.requestPath = requestPath;
            this.responsePath = responsePath;
            this.encoding = encoding;
            this.readBinary = readBinary;
            this.readFile = readFile;
            this.readFileAsync = readFileAsync;
        }

        internal static Mapping MapBinary(Method method, 
                                          PathString requestPath, 
                                          string responsePath, 
                                          Encoding encoding, 
                                          ReadBinary readBinary)
        {
            return new Mapping(method, 
                               ReadType.Binary,
                               requestPath,
                               responsePath, 
                               encoding, 
                               readBinary, 
                               null, 
                               null);
        }

        internal static Mapping MapFile(Method method, 
                                        PathString requestPath, 
                                        string responsePath, 
                                        ReadFile readFile)
        {
            return new Mapping(method, 
                               ReadType.File,
                               requestPath, 
                               responsePath, 
                               null, 
                               null, 
                               readFile, 
                               null);
        }

        internal static Mapping MapFileAsync(Method method, 
                                             PathString requestPath, 
                                             string responsePath,
                                             ReadFileAsync readFileAsync)
        {
            return new Mapping(method, 
                               ReadType.FileAsync,
                               requestPath,
                               responsePath, 
                               null, 
                               null, 
                               null, 
                               readFileAsync);
        }

        internal override void Setup(IApplicationBuilder app)
        {
            app.UseEndpoints(Build);
        }

        private void Build(IEndpointRouteBuilder builder)
        {
            switch (method)
            {
                case Method.Get: builder.MapGet(requestPath, Respond); break;
                case Method.Post: builder.MapPost(requestPath, Respond); break;
                case Method.Any: builder.Map(requestPath, Respond); break;
            }
        }

        private async Task Respond(HttpContext context) 
        {
            context.Response.StatusCode = (int)Status.SuccessCode.Ok;

            switch (readType)
            {
                case ReadType.Binary:
                    context.Response.WriteBytes(readBinary.Invoke(responsePath, encoding), 
                                                encoding);
                    break;

                case ReadType.File:
                    await context.Response.WriteAsync(readFile.Invoke(responsePath), false);
                    break;

                case ReadType.FileAsync:
                    await context.Response.WriteAsync(await readFileAsync.Invoke(responsePath), false);
                    break;
            }

            await context.Response.CompleteAsync();
        }
    }
}
