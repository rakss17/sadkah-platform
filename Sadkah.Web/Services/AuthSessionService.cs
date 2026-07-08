using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Sadkah.Web.Services
{
    public sealed class AuthSessionService : IAuthSessionService
    {
        public const string CookieName = "sadkah_auth_session";
        public const string ProtectorPurpose = "Sadkah.Web.AuthSession";

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJSRuntime jsRuntime;
        private readonly IDataProtector protector;
        private AuthResult? currentSession;

        public AuthSessionService(
            IHttpContextAccessor httpContextAccessor,
            IJSRuntime jsRuntime,
            IDataProtectionProvider dataProtectionProvider)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jsRuntime = jsRuntime;
            protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        public Task<string?> GetAccessTokenAsync()
        {
            return Task.FromResult(GetSession()?.AccessToken);
        }

        public Task<string?> GetRefreshTokenAsync()
        {
            return Task.FromResult(GetSession()?.RefreshToken);
        }

        public Task<string?> GetCurrentUserFullNameAsync()
        {
            return Task.FromResult(GetSession()?.FullName);
        }

        public Task<string?> GetCurrentUserEmailAsync()
        {
            return Task.FromResult(GetSession()?.Email);
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(GetSession()?.AccessToken));
        }

        public async Task SaveAsync(AuthResult authResult)
        {
            currentSession = authResult;
            await PersistAsync(authResult);
        }

        public async Task SaveTokensAsync(string accessToken, string refreshToken)
        {
            var session = GetSession() ?? new AuthResult();
            session.AccessToken = accessToken;
            session.RefreshToken = refreshToken;
            currentSession = session;

            await PersistAsync(session);
        }

        private async Task PersistAsync(AuthResult authResult)
        {
            await jsRuntime.InvokeVoidAsync("fetch", "/auth/session", new
            {
                method = "POST",
                headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                },
                body = JsonSerializer.Serialize(authResult),
                credentials = "same-origin"
            });
        }

        private AuthResult? GetSession()
        {
            if (currentSession is not null)
            {
                return currentSession;
            }

            var context = httpContextAccessor.HttpContext;
            if (context is null ||
                !context.Request.Cookies.TryGetValue(CookieName, out var cookieValue) ||
                string.IsNullOrWhiteSpace(cookieValue))
            {
                return null;
            }

            try
            {
                currentSession = JsonSerializer.Deserialize<AuthResult>(protector.Unprotect(cookieValue));
                return currentSession;
            }
            catch
            {
                return null;
            }
        }
    }
}
