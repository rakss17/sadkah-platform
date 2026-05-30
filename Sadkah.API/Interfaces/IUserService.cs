using Sadkah.API.Dtos.User;

namespace Sadkah.API.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
