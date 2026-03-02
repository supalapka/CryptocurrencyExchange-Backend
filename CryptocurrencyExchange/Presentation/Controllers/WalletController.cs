using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Application.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        [HttpGet("auth/get-wallet")]
        public async Task<ActionResult<List<WalletItem>>> GetFullWallet()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return await _walletService.GetFullWalletAsync(userId);
        }


        [HttpGet("auth/coin-amount/{symbol}")]
        public async Task<ActionResult<decimal>> GetCoinAmount(string symbol)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return await _walletService.GetCoinAmountAsync(userId, new CoinSymbol(symbol));
        }


        [HttpPost("auth/buy")]
        public async Task<ActionResult> Buy([FromBody] CoinTradeDto coinTradeDto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _walletService.BuyAsync(userId, coinTradeDto);

            return Ok();
        }


        [HttpPost("auth/sell")]
        public async Task<ActionResult> Sell([FromBody] CoinTradeDto coinTradeDto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _walletService.SellAsync(userId, coinTradeDto);

            return Ok();
        }


        public class SendCryptoModel
        {
            public string symbol { get; set; } = string.Empty;
            public decimal amount { get; set; }
            public int receiver { get; set; }
        }
    }
}
