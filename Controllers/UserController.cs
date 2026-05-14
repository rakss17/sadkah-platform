using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sadkah.Backend.Dtos.User;

namespace Sadkah.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null) return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password."));

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded) return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password."));

                var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

                return Ok(ApiResponse<NewUserDto>.SuccessResponse("Login successful.", new NewUserDto
                {
                    Email = user.Email ?? string.Empty,
                    FullName = user.FirstName + " " + user.LastName,
                    AccessToken = _tokenService.CreateToken(user),
                    RefreshToken = refreshToken.Token
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error during login."));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            try
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

                if (createdUser.Succeeded)
                {
                    var addToRole = await _userManager.AddToRoleAsync(user, registerDto.Role.ToString() ?? UserRole.Unassigned.ToString());
                    if (addToRole.Succeeded)
                    {
                        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

                        return Ok(ApiResponse<NewUserDto>.SuccessResponse("User registered successfully.", new NewUserDto
                        {
                            Email = user.Email ?? string.Empty,
                            FullName = user.FirstName + " " + user.LastName,
                            AccessToken = _tokenService.CreateToken(user),
                            RefreshToken = refreshToken.Token
                        }));
                    }
                    else
                    {
                        Console.WriteLine($"Error adding user to role: {string.Join(", ", addToRole.Errors.Select(e => e.Description))}");
                        return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while adding user to role."));
                    }
                }
                else
                {
                    Console.WriteLine($"Error creating user: {string.Join(", ", createdUser.Errors.Select(e => e.Description))}");
                    return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while creating user."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error during registration."));
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            try
            {
                var (accessToken, newRefresh) = await _tokenService.RefreshAsync(refreshToken);

                return Ok(ApiResponse<object>.SuccessResponse("Token refreshed successfully.", new
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefresh.Token
                }));
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(ApiResponse<object>.FailResponse("Invalid or expired refresh token."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token refresh: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error during token refresh."));
            }
        }
    }
}
