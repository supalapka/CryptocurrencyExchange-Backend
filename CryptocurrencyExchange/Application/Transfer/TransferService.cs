using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Application.Transfers
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferVerificationOutboxRepository _outboxRepository;
        private readonly ILogger<TransferService> _logger;

        public TransferService(
            ITransferRepository transferRepository,
            IWalletItemRepository walletItemRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITransferVerificationOutboxRepository outboxRepository,
            ILogger<TransferService> logger)
        {
            _transferRepository = transferRepository;
            _walletItemRepository = walletItemRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _outboxRepository = outboxRepository;
            _logger = logger;
        }

        public async Task<int> InitiateAsync(int senderId, InitiateTransferDto dto)
        {
            var receiver = await _userRepository.GetByEmailAsync(dto.ReceiverEmail)
                ?? throw new UserNotFoundException();

            if (receiver.Id == senderId)
                throw new SelfTransferException();

            var symbol = new CoinSymbol(dto.CoinSymbol);

            var senderItem = await _walletItemRepository.GetWithUserAsync(senderId, symbol)
                ?? throw new WalletItemNotFoundException();

            if (senderItem.Amount.Value < dto.Amount)
                throw new InsufficientFundsException();

            var code = GenerateCode();
            Transfer transfer = null;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                transfer = Transfer.Create(senderId, receiver.Id, symbol, dto.Amount, code);
                await _transferRepository.AddAsync(transfer);
                await _outboxRepository.AddAsync(new TransferVerificationOutbox(senderItem.User.Email, code));
            });

            _logger.LogInformation("Transfer {TransferId} initiated by user {SenderId}", transfer.Id, senderId);

            return transfer.Id;
        }

        public async Task ConfirmAsync(int senderId, ConfirmTransferDto dto)
        {
            var codeVo = new VerificationCode(dto.VerificationCode);

            var transfer = await _transferRepository.GetPendingByIdAndSenderAsync(dto.TransferId, senderId)
                ?? throw new TransferNotFoundException();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var (senderItem, receiverItem) = await _walletItemRepository
                    .GetForTransferAsync(senderId, transfer.ReceiverId, transfer.Symbol);

                if (senderItem is null)
                    throw new WalletItemNotFoundException();

                if (receiverItem is null)
                {
                    receiverItem = new WalletItem(transfer.ReceiverId, transfer.Symbol);
                    await _walletItemRepository.AddAsync(receiverItem);
                }

                transfer.Execute(senderItem, receiverItem, codeVo);
            });

            _logger.LogInformation("Transfer {TransferId} completed by user {SenderId}", dto.TransferId, senderId);
        }

        private static string GenerateCode() => Random.Shared.Next(100_000, 1_000_000).ToString();
    }
}
