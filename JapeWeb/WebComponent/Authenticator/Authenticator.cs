using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public class Authenticator : IWebComponent
    {
        public const int ByteSize = 8;
        public const int SaltSize = 128;
        public const int HashSize = 256;
        public const int Iterations = 65536;

        private readonly Signup signup;
        private Success<SignupData> signupSuccess;

        private readonly Login login;
        private Success<LoginData> loginSuccess;

        private readonly Middleware.RequestLookup<string> getRequestUser;
        private readonly Middleware.RequestLookup<string> getRequestPassword;

        public delegate Task<SignupData> Signup(string user, string password, string salt);
        public delegate Task<LoginData> Login(string user);
        public delegate void Success<in T>(Middleware.Request request, T data);

        internal Authenticator(Signup signup,
                               Login login,
                               Middleware.RequestLookup<string> getRequestUser, 
                               Middleware.RequestLookup<string> getRequestPassword)
        {
            this.signup = signup;
            this.login = login;
            this.getRequestUser = getRequestUser;
            this.getRequestPassword = getRequestPassword;
        }

        public void SignupSuccess(Success<SignupData> signupSuccess) { this.signupSuccess = signupSuccess; }
        public void LoginSuccess(Success<LoginData> loginSuccess) { this.loginSuccess = loginSuccess; }

        internal async Task<Middleware.Result> ResponseSignup(Middleware.Request request)
        {
            string user = getRequestUser(request);
            string password = getRequestPassword(request);

            byte[] hash = Hash(password, out byte[] salt);

            SignupData data;
            try
            {
                data = await signup(user, Convert.ToBase64String(hash), Convert.ToBase64String(salt));
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }

            if (data == null)
            {
                Log.Write("Null Signup Data");
                return await request.Abort(Status.ErrorCode.ServerError);
            }

            signupSuccess?.Invoke(request, data);

            return await request.Complete(Status.SuccessCode.Ok, data.Id);
        }

        internal async Task<Middleware.Result> ResponseLogin(Middleware.Request request)
        {
            LoginData data;
            try
            {
                data = await login(getRequestUser(request));
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }

            if (data == null)
            {
                Log.Write("Null Login Data");
                return await request.Abort(Status.ErrorCode.ServerError);
            }

            if (!CompareHash(Convert.FromBase64String(data.Password), getRequestPassword(request), Convert.FromBase64String(data.Salt)))
            {
                return await request.Abort(Status.ErrorCode.NotFound);
            }

            loginSuccess?.Invoke(request, data);

            return await request.Complete(Status.SuccessCode.Ok, data.Id);
        }

        private static bool CompareHash(byte[] hash, string value, byte[] salt) => hash.SequenceEqual(HashSalt(value, salt));

        private static byte[] HashSalt(string value, byte[] salt) => KeyDerivation.Pbkdf2(value, 
                                                                                          salt, 
                                                                                          KeyDerivationPrf.HMACSHA256, 
                                                                                          Iterations, 
                                                                                          HashSize / ByteSize);

        private static byte[] Hash(string value, out byte[] salt)
        {
            salt = new byte[SaltSize / ByteSize];
            using RNGCryptoServiceProvider encrypter = new();
            encrypter.GetNonZeroBytes(salt);
            return HashSalt(value, salt);
        }

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
