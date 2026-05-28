namespace Sadkah.Web.Interfaces
{
    public interface IAuthSessionService
    {
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task<string?> GetCurrentUserFullNameAsync();
        Task<bool> IsAuthenticatedAsync();
        Task SaveAsync(AuthResult authResult);
        Task SaveTokensAsync(string accessToken, string refreshToken);
    }
}
