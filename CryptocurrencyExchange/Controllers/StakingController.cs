using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [Route("staking")]
    //[Authorize]
    [ApiController]
    public class StakingController : ControllerBase
    {
        private readonly IStakingService _stakingService;

        public StakingController(IStakingService stakingService)
        {
            _stakingService = stakingService;
        }

        [HttpGet("available-coins")]
        public async Task<IActionResult> GetCoins()
        {
            return Ok(await _stakingService.GetCoinsAsync());
        }


        [HttpGet("user-coins")]
        [Authorize]
        public IActionResult GetUserStakings()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return Ok(_stakingService.GetStakingsByUser(userId));
        }


        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateStaking(StakingInput input)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            try
            {
                await _stakingService.CreateUserStaking(userId, input.stakingCoinId, input.Amount, input.DurationInMonth);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }


        [HttpGet("check")]
        public void CheckIfExpired() => _stakingService.CheckForExpiredStakings();


        public class StakingInput
        {
            public int stakingCoinId { get; set; }
            public decimal Amount { get; set; }
            public int DurationInMonth { get; set; }
        }
    }
}
