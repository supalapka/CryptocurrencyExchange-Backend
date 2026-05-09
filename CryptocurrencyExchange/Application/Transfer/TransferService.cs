using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Application.Transfers
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferVerificationOutboxRepository _outboxRepository;
        private readonly ITransferIdempotentRequestRepository _idempotentRequestRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<TransferService> _logger;

        public TransferService(
            ITransferRepository transferRepository,
            IWalletItemRepository walletItemRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITransferVerificationOutboxRepository outboxRepository,
            ITransferIdempotentRequestRepository idempotentRequestRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<TransferService> logger)
        {
            _transferRepository = transferRepository;
            _walletItemRepository = walletItemRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _outboxRepository = outboxRepository;
            _idempotentRequestRepository = idempotentRequestRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<int> InitiateAsync(int senderId, InitiateTransferDto dto, string idempotencyKey)
        {
            var existing = await _idempotentRequestRepository.FindAsync(idempotencyKey, senderId);
            if (existing is not null && !existing.IsExpired && existing.TransferId.HasValue)
                return existing.TransferId.Value;

            var receiver = await _userRepository.GetByEmailAsync(dto.ReceiverEmail)
                ?? throw new UserNotFoundException();

            var symbol = new CoinSymbol(dto.CoinSymbol);

            var senderItem = await _walletItemRepository.GetWithUserAsync(senderId, symbol);
            if (senderItem is null || !senderItem.HasSufficientBalance(dto.Amount))
                throw new InsufficientFundsException();

            var code = VerificationCode.Generate();
            var idempotentRequest = new TransferIdempotentRequest(idempotencyKey, senderId);
            Transfer transfer = null;

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await _idempotentRequestRepository.AddAsync(idempotentRequest);
                    transfer = Transfer.Create(senderId, receiver.Id, symbol, dto.Amount, code);
                    await _transferRepository.AddAsync(transfer);
                    await _outboxRepository.AddAsync(new TransferVerificationOutbox(senderItem.User.Email, code.Value));
                    await _unitOfWork.CommitAsync();
                    idempotentRequest.SetTransferId(transfer.Id);
                });
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
            {
                var conflict = await _idempotentRequestRepository.FindAsync(idempotencyKey, senderId);
                if (conflict?.TransferId.HasValue == true)
                    return conflict.TransferId.Value;
                throw;
            }

            _logger.LogInformation("Transfer {TransferId} initiated by user {SenderId}", transfer!.Id, senderId);

            return transfer.Id;
        }

        public async Task ConfirmAsync(int senderId, ConfirmTransferDto dto)
        {
            var codeVo = new VerificationCode(dto.VerificationCode);

            var transfer = await _transferRepository.GetPendingByIdAndSenderAsync(dto.TransferId, senderId);

            if (transfer is null)
            {
                var completed = await _transferRepository.GetCompletedByIdAndSenderAsync(dto.TransferId, senderId);
                if (completed is not null)
                    return;

                throw new TransferNotFoundException();
            }

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

            var senderEmail = await _userRepository.GetEmailByIdAsync(senderId);
            var receiverEmail = await _userRepository.GetEmailByIdAsync(transfer.ReceiverId);
            await _publishEndpoint.Publish(new TransferCompletedEvent(
                transfer.Id, senderId, senderEmail, transfer.ReceiverId, receiverEmail, transfer.Amount, transfer.Symbol.Value));

            _logger.LogInformation("Transfer {TransferId} completed by user {SenderId}", dto.TransferId, senderId);
        }

        private static bool IsDuplicateKeyException(DbUpdateException ex) =>
            ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }
}
