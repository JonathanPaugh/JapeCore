using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public class AuthenticatorOptions
    {
        public PathString RequestPath { get; set; } = "/authenticate";
        public PathString SignupPath { get; set; } = "/signup";
        public PathString LoginPath { get; set; } = "/login";

        public static AuthenticatorOptions Default => new();
    }
}
