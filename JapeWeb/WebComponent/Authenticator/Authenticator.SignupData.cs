namespace JapeWeb
{
    public partial class Authenticator
    {
        public class SignupData
        {
            public string Id { get; }
            public object[] Args { get; }

            public SignupData(string id, params object[] args)
            {
                Id = id;
                Args = args;
            }
        }
    }
}