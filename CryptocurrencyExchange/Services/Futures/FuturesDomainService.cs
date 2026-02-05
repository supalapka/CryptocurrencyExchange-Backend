using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Services.Futures
{
    public class FuturesDomainService : IFuturesDomainService
    {
        public Future OpenPosition(FutureDto futureDto, WalletItem usdtItem)
        {
            EnsureSufficientBalance(usdtItem.Amount, futureDto.Margin);

            var future = CreateFuture(futureDto, usdtItem.UserId);

            DebitMargin(future.Margin, usdtItem);

            return future;
        }
       
        public void ClosePosition(Future position, WalletItem usdtItem, decimal pnl)
        {
            MarkPositionAsCompleted(position);

            usdtItem.Amount = CalculateBalanceAfterClose(usdtItem.Amount, position.Margin, pnl);
        }

        public void LiquidatePosition(Future position)
        {
            MarkPositionAsCompleted(position);
        }




        private void DebitMargin(decimal futureMargin, WalletItem usdt)
        {
            usdt.Amount = Math.Round(usdt.Amount - futureMargin, 2);
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

        private decimal CalculateBalanceAfterClose(decimal currentBalance, decimal margin, decimal pnl)
        {
            return Math.Round(currentBalance + margin + pnl, 2);
        }

    }
}
