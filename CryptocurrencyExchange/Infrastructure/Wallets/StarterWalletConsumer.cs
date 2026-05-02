using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Wallets
{
    public class StarterWalletConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IWalletItemRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StarterWalletConsumer(IWalletItemRepository walletRepository, IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var walletItem = new WalletItem(context.Message.UserId, new CoinSymbol(CoinSymbol.Usdt.Value));
            walletItem.AddAmount(5000);

            try
            {
                await _walletRepository.AddAsync(walletItem);
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
            {
                // another consumer instance already created the wallet, don't need to throw an exception
            }
        }

        private static bool IsDuplicateKeyException(DbUpdateException ex) =>
            ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }
}
