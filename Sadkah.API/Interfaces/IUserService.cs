using Sadkah.API.Dtos.User;

namespace Sadkah.API.Interfaces
{
    public interface IUserService
    {
        Task<NewUserDto?> LoginAsync(LoginDto loginDto);
        Task<ServiceResult<NewUserDto>> RegisterAsync(RegisterDto registerDto);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
