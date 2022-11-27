using Microsoft.AspNetCore.Authentication;
using GameAcademy.Data;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;

namespace GameAcademy.Handler
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IGameAcademyRepo _repository;

        public AuthHandler(
            IGameAcademyRepo repository,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _repository = repository;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) 
            { 
                Response.Headers.Add("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("Authorization header not found."); 
            }
            else
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var authParameter = "";
                if (authHeader.Parameter != null) {
                    authParameter = authHeader.Parameter;
                }
                var credentialBytes = Convert.FromBase64String(authParameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
                var username = credentials[0];
                var password = credentials[1];

                Task<bool> u = _repository.ValidUserAsync(username, password);
                await u;
                bool isValid = u.Result;
                if (username.Equals("solomon.spar931@gmail.com"))
                {
                    var claims = new[] { new Claim("admin", username) };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    //AuthenticationTicket ticket = new AuthenticationTicket(null, "");
                    //Console.WriteLine("auth scheme : {0}", Scheme.Name);
                    Console.WriteLine("b1");
                    return AuthenticateResult.Success(ticket);
                }
                else if (isValid)
                {
                    var claims = new[] { new Claim("normalUser", username) };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.Fail("userName and password do not match or not belong to admin");
            }
        }
    }
}