using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            await _authService.RegisterAsync(userDto.Email, userDto.Password);

            return Ok($"{userDto.Email} successfully registered");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto userDto)
        {
            var token = await _authService.LoginAsync(userDto.Email, userDto.Password);

            return Ok(token);
        }

        [Authorize]
        [HttpGet("email")]
        public async Task<ActionResult<string>> GetUserEmail()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string email = await _authService.GetEmailByIdAsync(userId);

            return Ok(email);
        }
    }
}
