namespace JapeWeb
{
    public partial class Middleware
    {
        public class Result : JapeHttp.Request.Result
        {
            internal bool Prevented { get; }
            internal bool Skipped { get; private init; }

            internal static Result Prevent => new(true);
            internal static Result Next => new(false);
            internal static Result Skip => new(false)
            {
                Skipped = true
            };

            private Result(bool prevented)
            {
                Prevented = prevented;
            }

            
        }
    }
}