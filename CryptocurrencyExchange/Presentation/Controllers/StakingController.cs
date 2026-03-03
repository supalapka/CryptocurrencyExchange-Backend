using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Route("staking")]
    public class StakingController : ApiControllerBase
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
            int userId = UserId;

            return Ok(_stakingService.GetStakingsByUser(userId));
        }


        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateStaking(StakingInput input)
        {
            try
            {
                await _stakingService.CreateUserStaking(UserId, input.stakingCoinId, input.Amount, input.DurationInMonth);
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
