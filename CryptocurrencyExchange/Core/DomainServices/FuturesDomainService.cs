using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Services.Futures;

namespace CryptocurrencyExchange.Core.Domain
{
    public class FuturesDomainService : IFuturesDomainService
    {
        public Future OpenPosition(FutureDto futureDto, WalletItem usdtItem)
        {
            EnsureSufficientBalance(usdtItem.Amount, futureDto.Margin);
            var future = CreateFuture(futureDto, usdtItem.UserId);
            usdtItem.RemoveAmount((future.Margin));

            return future;
        }
       
        public void ClosePosition(Future position, WalletItem usdtItem, decimal pnl)
        {
            MarkPositionAsCompleted(position);
            usdtItem.AddAmount(CalculatePayoutAmount(position.Margin, pnl));
        }

        public void LiquidatePosition(Future position)
        {
            MarkPositionAsCompleted(position);
        }

        private void MarkPositionAsCompleted(Future position)
        {
            if (position.IsCompleted)
                throw new InvalidOperationException("Position is already completed.");
            position.IsCompleted = true;
        }

        private void EnsureSufficientBalance(decimal balance, decimal margin)
        {
            if (balance < margin)
                throw new InsufficientFundsException();
        }

        private Future CreateFuture(FutureDto dto, int userId) =>
          new()
          {
              Symbol = dto.Symbol,
              EntryPrice = dto.EntryPrice,
              Margin = dto.Margin,
              UserId = userId,
              IsCompleted = false,
              Leverage = dto.Leverage,
              Position = dto.Position
          };

        private decimal CalculatePayoutAmount(decimal margin, decimal pnl)
        {
            return Math.Round(margin + pnl, 2);
        }
    }
}
