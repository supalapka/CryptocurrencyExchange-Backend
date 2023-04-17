using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Controllers
{
    public class WalletController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWalletService _walletService;

        public WalletController(DataContext dataContext, IWalletService walletService)
        {
            _dataContext = dataContext;
            _walletService = walletService;
        }


        [Authorize]
        [HttpGet("auth/get-wallet")]
        public async Task<ActionResult<List<WalletItem>>> GetFullWallet()
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            return await _walletService.GetFullWalletAsync(userId);
        }


        [Authorize]
        [HttpPost("auth/coin-amount")]
        public async Task<ActionResult<double>> GetCoinAmount([FromBody] CoinAmountRequest request)
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            return await _walletService.GetCoinAmountAsync(userId, request.Symbol);
        }


        [HttpPost("auth/buy")]
        [Authorize]
        public async Task<ActionResult> Buy([FromBody] BuySellCryptoModel buyCryptoModel)
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);
            var usdAmount = buyCryptoModel.Amount;

            try
            {
                await _walletService.BuyAsync(userId, buyCryptoModel.CoinSymbol, usdAmount);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            return Ok();
        }


        [HttpPost("auth/sell")]
        [Authorize]
        public async Task<ActionResult> Sell([FromBody] BuySellCryptoModel buyCryptoModel)
        {
            var coinAmount = buyCryptoModel.Amount;
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            try
            {
                await _walletService.SellAsync(userId, buyCryptoModel.CoinSymbol, coinAmount);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            return Ok();
        }


        [HttpPost("auth/send")]
        [Authorize]
        public async Task<IActionResult> SendCrypto([FromBody] SendCryptoModel model)
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            try
            {
                await _walletService.SendCryptoAsync(userId, model.symbol, model.amount, model.receiver);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }

            return Ok();
        }

        [HttpGet("price/{symbol}")]
        public async Task<decimal> GetPrice(string symbol)
        {
            return await _walletService.GetPrice(symbol);
        }

        public class SendCryptoModel
        {
            public string symbol { get; set; } = string.Empty;
            public double amount { get; set; }
            public int receiver { get; set; }
        }

        public class BuySellCryptoModel
        {
            public string CoinSymbol { get; set; } = string.Empty;
            public double Amount { get; set; }
            //if buy -> usd amount.
            //if sell -> coin amount
        }

        public class CoinAmountRequest
        {
            public string Symbol { get; set; }
        }
    }
}
