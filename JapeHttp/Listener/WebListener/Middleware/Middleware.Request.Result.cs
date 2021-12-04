namespace JapeHttp
{
    public partial class Middleware
    {
        public partial class Request
        {
            public new class Result : JapeHttp.Request.Result
            {
                internal bool Prevented { get; }
                internal bool Skipped { get; private init; }

                internal static Result Prevent => new(true);
                internal static Result Next => new(false);

                private Result(bool prevented)
                {
                    Prevented = prevented;
                }

                internal static Result Skip()
                {
                    return new Result(false)
                    {
                        Skipped = true
                    };
                }
            }
        }
    }
}