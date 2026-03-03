using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using MassTransit;

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
            await _walletRepository.AddAsync(walletItem);
            await _unitOfWork.CommitAsync();
        }
    }
}
