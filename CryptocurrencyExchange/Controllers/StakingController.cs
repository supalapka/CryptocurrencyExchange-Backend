using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [Route("staking")]
    [Authorize]
    [ApiController]
    public class StakingController : ControllerBase
    {
        private readonly IStakingService _stakingService;

        public StakingController(IStakingService stakingService)
        {
            _stakingService = stakingService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateStaking(StakingInput input)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            await _stakingService.CreateStakingCoin(userId, input.stakingCoinId, input.Amount, input.DurationInDays);
            return Ok();
        }

        public class StakingInput
        {
            public int stakingCoinId { get; set; }
            public float Amount { get; set; }
            public int DurationInDays { get; set; }
        }
    }
}
