using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Wallet;
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
        public async Task<ActionResult> Buy([FromBody] BuySellCryptoModel buyCryptoModel)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            decimal usdAmount = (decimal)buyCryptoModel.Amount;

            try
            {
                await _walletService.BuyAsync(userId, buyCryptoModel.CoinSymbol, usdAmount);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            return Ok();
        }


        [HttpPost("auth/sell")]
        public async Task<ActionResult> Sell([FromBody] BuySellCryptoModel buyCryptoModel)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var coinAmount = buyCryptoModel.Amount;

            try
            {
                await _walletService.SellAsync(userId, buyCryptoModel.CoinSymbol, coinAmount);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
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

        public class BuySellCryptoModel
        {
            public string CoinSymbol { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            //if buy -> usd amount.
            //if sell -> coin amount
        }

    }
}
