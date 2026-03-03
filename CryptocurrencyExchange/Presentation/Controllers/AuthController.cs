using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            await _authService.RegisterAsync(userDto.Email, userDto.Password);

            return Ok($"{userDto.Email} successfully registered");
        }

        [AllowAnonymous]
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
            string email = await _authService.GetEmailByIdAsync(UserId);

            return Ok(email);
        }
    }
}
