namespace Sadkah.Web.Services
{
    public sealed class AuthSessionService : IAuthSessionService
    {
        private const string AccessTokenKey = "sadkah_access_token";
        private const string RefreshTokenKey = "sadkah_refresh_token";
        private const string UserEmailKey = "sadkah_user_email";
        private const string UserFullNameKey = "sadkah_user_full_name";

        private readonly IJSRuntime jsRuntime;

        public AuthSessionService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
        }

        public async Task<string?> GetCurrentUserFullNameAsync()
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserFullNameKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetAccessTokenAsync();
            return !string.IsNullOrWhiteSpace(token);
        }

        public async Task SaveAsync(AuthResult authResult)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, authResult.AccessToken);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, authResult.RefreshToken);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", UserEmailKey, authResult.Email);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", UserFullNameKey, authResult.FullName);
        }
    }
}
