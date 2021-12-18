namespace JapeWeb
{
    public partial class Authenticator
    {
        public class LoginData
        {
            public string Id { get; }
            public string User { get; }
            public string Password { get; }
            public string Salt { get; }
            public object[] Args { get; }

            public LoginData(string id, string user, string password, string salt, params object[] args)
            {
                Id = id;
                User = user;
                Password = password;
                Salt = salt;
                Args = args;
            }
        }
    }
}