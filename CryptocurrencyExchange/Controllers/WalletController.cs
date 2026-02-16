using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Infrastructure.Persistence;
using CryptocurrencyExchange.Services.WalletTrade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWalletService _walletService;

        public WalletController(DataContext dataContext, IWalletService walletService)
        {
            _dataContext = dataContext;
            _walletService = walletService;
        }


        [HttpGet("auth/get-wallet")]
        public async Task<ActionResult<List<WalletItem>>> GetFullWallet()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

            return await _walletService.GetFullWalletAsync(userId);
        }


        [HttpGet("auth/coin-amount/{symbol}")]
        public async Task<ActionResult<decimal>> GetCoinAmount(string symbol)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

            return await _walletService.GetCoinAmountAsync(userId, symbol);
        }


        [HttpPost("auth/buy")]
        public async Task<ActionResult> Buy([FromBody] CoinTradeDto coinTradeDto)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            coinTradeDto.UserId = userId;

            await _walletService.BuyAsync(coinTradeDto);

            return Ok();
        }


        [HttpPost("auth/sell")]
        public async Task<ActionResult> Sell([FromBody] CoinTradeDto coinTradeDto)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            coinTradeDto.UserId = userId;

            await _walletService.SellAsync(coinTradeDto);

            return Ok();
        }


        //[HttpPost("auth/send")]
        //public async Task<IActionResult> SendCrypto([FromBody] SendCryptoModel model)
        //{
        //    int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

        //    try
        //    {
        //        await _walletService.SendCryptoAsync(userId, model.symbol, model.amount, model.receiver);
        //    }
        //    catch (Exception ex) { return BadRequest(ex.Message); }

        //    return Ok();
        //}


        public class SendCryptoModel
        {
            public string symbol { get; set; } = string.Empty;
            public decimal amount { get; set; }
            public int receiver { get; set; }
        }
    }
}
