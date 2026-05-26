namespace Sadkah.Web.Interfaces
{
    public interface IAuthSessionService
    {
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetCurrentUserFullNameAsync();
        Task<bool> IsAuthenticatedAsync();
        Task SaveAsync(AuthResult authResult);
    }
}
