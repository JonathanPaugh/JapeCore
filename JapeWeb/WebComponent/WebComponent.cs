using Microsoft.AspNetCore.Builder;

namespace JapeWeb
{
    public abstract class WebComponent
    {
        internal abstract void Setup(IApplicationBuilder app);
    }
}
