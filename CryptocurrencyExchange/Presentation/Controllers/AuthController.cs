using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            var email = new Email(userDto.Email);
            await _authService.RegisterAsync(email, new Password(userDto.Password));

            return Ok($"{email} successfully registered");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto userDto)
        {
            var token = await _authService.LoginAsync(new Email(userDto.Email), new Password(userDto.Password));

            return Ok(token);
        }

        [Authorize]
        [HttpGet("email")]
        public async Task<ActionResult<string>> GetUserEmail()
        {
            string email = await _userService.GetEmailByIdAsync(UserId);

            return Ok(email);
        }
    }
}
