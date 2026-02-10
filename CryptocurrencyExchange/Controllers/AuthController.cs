using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CryptocurrencyExchange.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly IAuthService _authService;
        private readonly DataContext _dataContext;

        public AuthController(IConfiguration configuration, DataContext dataContext, IAuthService authService)
        {
            _configuration = configuration;
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
        public async Task<ActionResult<User>> Login(UserDto userDto)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null)
                return BadRequest("User not found");

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Wrond password");

            string jwt = CreateToken(user);
            return Ok(jwt);
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


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}
