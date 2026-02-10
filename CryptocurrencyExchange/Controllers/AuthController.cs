using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        private readonly DataContext _dataContext;

        public AuthController(DataContext dataContext, IAuthService authService)
        {
            _dataContext = dataContext;
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
        public async Task<ActionResult<User>> GetUserEmail()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

            var user = _dataContext.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user == null)
                return BadRequest("Invalid or miss jwt");

            return Ok(user.Email);
        }
    }
}
