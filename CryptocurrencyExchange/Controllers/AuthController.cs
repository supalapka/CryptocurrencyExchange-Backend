using CryptocurrencyExchange.Services.Auth;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet("email")]
        public async Task<ActionResult<string>> GetUserEmail()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            string email = await _authService.GetEmailByIdAsync(userId);

            return Ok(email);
        }
    }
}
