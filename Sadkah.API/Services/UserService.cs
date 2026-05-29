using Microsoft.AspNetCore.Identity;
using Sadkah.API.Dtos.User;

namespace Sadkah.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<NewUserDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return null;

            return await CreateNewUserDtoAsync(user);
        }

        public async Task<ServiceResult<NewUserDto>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName ?? string.Empty,
                LastName = registerDto.LastName ?? string.Empty,
                Role = registerDto.Role ?? UserRole.Unassigned
            };

            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

            if (!createdUser.Succeeded)
            {
                var errors = createdUser.Errors.Select(e => e.Description);
                return ServiceResult<NewUserDto>.Failure("Internal server error while creating user.", errors);
            }

            var roleName = (registerDto.Role ?? UserRole.Unassigned).ToString();
            var addToRole = await _userManager.AddToRoleAsync(user, roleName);

            if (!addToRole.Succeeded)
            {
                var errors = addToRole.Errors.Select(e => e.Description);
                return ServiceResult<NewUserDto>.Failure("Internal server error while adding user to role.", errors);
            }

            return ServiceResult<NewUserDto>.Success(await CreateNewUserDtoAsync(user));
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var (accessToken, newRefresh) = await _tokenService.RefreshAsync(refreshToken);
            return (accessToken, newRefresh.Token);
        }

        private async Task<NewUserDto> CreateNewUserDtoAsync(User user)
        {
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return new NewUserDto
            {
                Email = user.Email ?? string.Empty,
                FullName = user.FirstName + " " + user.LastName,
                AccessToken = _tokenService.CreateToken(user),
                RefreshToken = refreshToken.Token
            };
        }
    }
}
