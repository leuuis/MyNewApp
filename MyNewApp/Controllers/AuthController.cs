using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyNewApp.DTOs;
using MyNewApp.Services.Interfaces;
using MyNewApp.Helpers;

namespace MyNewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger _logger;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService, ILogger logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequestDto loginDto, [FromServices] IValidator<LoginRequestDto> validator)
        {
            SafeLogger.LogInfoSafe(_logger, "Login attempt", loginDto);

            var validationResult = await validator.ValidateAsync(loginDto, options => options.IncludeRuleSets("Login"));
    
            if (!validationResult.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
            }
            
            var user = await _authService.ValidateUserAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
