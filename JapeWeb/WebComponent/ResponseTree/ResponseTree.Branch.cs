using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class ResponseTree
    {
        internal class Branch
        {
            internal readonly PathString requestPath;
            internal readonly List<Branch> branches = new();

            internal Middleware Middleware { get; private set; }

            internal Branch(PathString requestPath, Middleware middleware = null)
            {
                this.requestPath = requestPath;
                Middleware = middleware;
            }

            private void InsertNextPathSegment(PathString path, Middleware middleware)
            {
                PathString branchPath = path.Pop(out PathString remaining);
                
                Branch branch = new(branchPath, string.IsNullOrEmpty(remaining) ? middleware : null);
                branches.Add(branch);

                if (!string.IsNullOrEmpty(remaining))
                {
                    branch.Insert(remaining, middleware);
                }
            }

            internal void Insert(PathString path, Middleware middleware)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Middleware = middleware;
                    return;
                }

                foreach (Branch child in branches)
                {
                    if (path.StartsWithSegments(child.requestPath, out PathString remainingPath))
                    {
                        child.Insert(remainingPath, middleware);
                        return;
                    }
                }

                InsertNextPathSegment(path, middleware);
            }

            internal async Task<Result> Respond(PathString requestPath, HttpContext context)
            {
                if (!requestPath.StartsWithSegments(this.requestPath, out PathString remainingPath))
                {
                    return Result.WrongPath;
                }

                if (string.IsNullOrEmpty(remainingPath))
                { 
                    if (Middleware == null) { return Result.NotFound; }
                    Middleware.Result middlewareResult = await Middleware.InvokeExternal(Middleware, context);
                    return Result.Found(middlewareResult);
                }

                foreach (Branch child in branches)
                {
                    Result result = await child.Respond(remainingPath, context);
                    if (result.Complete) { return result; }
                }

                return Result.NotFound;
            }

            internal void WriteChildren(Action<string> write, string previous)
            {
                string next = previous + requestPath;
                write(next);
                foreach (Branch child in branches)
                {
                    child.WriteChildren(write, next);
                }
            }
        }
    }
}