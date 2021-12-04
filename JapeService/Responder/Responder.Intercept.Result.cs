using JapeHttp;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public partial class Intercept
        {
            public new class Result : Request.Result
            {
                public bool Intercepted { get; }

                internal Result(bool intercepted)
                {
                    Intercepted = intercepted;
                }
            }
        }
    }
}