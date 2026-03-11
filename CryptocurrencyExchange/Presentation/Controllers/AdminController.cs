using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Route("admin")]
    [Authorize]
    public class AdminController : ApiControllerBase
    {
        private readonly IStakingPromotionService _stakingPromotionService;

        public AdminController(IStakingPromotionService stakingPromotionService)
        {
            _stakingPromotionService = stakingPromotionService;
        }

        [HttpPost("send-staking-promotion")]
        public async Task<IActionResult> SendStakingPromotionEmails()
        {
             await _stakingPromotionService.SendPromotionEmailsAsync();
            return Ok();
        }
    }
}
