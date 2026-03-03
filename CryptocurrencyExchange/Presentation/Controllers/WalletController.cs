using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Application.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Authorize]
    public class WalletController : ApiControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        [HttpGet("auth/get-wallet")]
        public async Task<ActionResult<List<WalletItem>>> GetFullWallet()
        {
            int userId = UserId;

            return await _walletService.GetFullWalletAsync(userId);
        }


        [HttpGet("auth/coin-amount/{symbol}")]
        public async Task<ActionResult<decimal>> GetCoinAmount(string symbol)
        {
            return await _walletService.GetCoinAmountAsync(UserId, new CoinSymbol(symbol));
        }


        [HttpPost("auth/buy")]
        public async Task<ActionResult> Buy([FromBody] CoinTradeDto coinTradeDto)
        {
            await _walletService.BuyAsync(UserId, coinTradeDto);

            return Ok();
        }


        [HttpPost("auth/sell")]
        public async Task<ActionResult> Sell([FromBody] CoinTradeDto coinTradeDto)
        {
            await _walletService.SellAsync(UserId, coinTradeDto);

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
