using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sadkah.API.Dtos.User;
using Microsoft.AspNetCore.RateLimiting;

namespace Sadkah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _userService.LoginAsync(loginDto);

                if (user == null) return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password."));

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse("Login successful.", user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error during login."));
            }
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _userService.RegisterAsync(registerDto);

                if (!result.Succeeded)
                {
                    Console.WriteLine($"Error during registration: {JsonSerializer.Serialize(result.Errors)}");
                    return StatusCode(500, ApiResponse<object>.FailResponse(result.ErrorMessage));
                }

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse("User registered successfully.", result.Data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error during registration."));
            }
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshToken)
        {
            try
            {
                var (accessToken, newRefreshToken) = await _userService.RefreshTokenAsync(refreshToken.RefreshToken);

                return Ok(ApiResponse<object>.SuccessResponse("Token refreshed successfully.", new
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
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
