using Microsoft.AspNetCore.Mvc;
using MyNewApp.DTOs;
using MyNewApp.Services.Interfaces;

namespace MyNewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequestDto loginDto)
        {
            var user = await _authService.ValidateUserAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
