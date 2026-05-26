namespace Sadkah.Web.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResult>> LoginAsync(LoginRequest request);
        Task<ServiceResult<AuthResult>> SignupAsync(SignupRequest request);
    }
}
