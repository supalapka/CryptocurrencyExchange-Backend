using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.Models
{
    public enum TransferStatus { Pending, Completed }

    public class Transfer
    {
        public int Id { get; private set; }
        public int SenderId { get; private set; }
        public int ReceiverId { get; private set; }
        public CoinSymbol Symbol { get; private set; }
        public decimal Amount { get; private set; }
        public VerificationCode Code { get; private set; }
        public TransferStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Transfer() { }

        public static Transfer Create(int senderId, int receiverId, CoinSymbol symbol, decimal amount, string verificationCode)
        {
            if (senderId == receiverId)
                throw new SelfTransferException();

            return new Transfer
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Symbol = symbol,
                Amount = amount,
                Code = new VerificationCode(verificationCode),
                Status = TransferStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Execute(WalletItem senderItem, WalletItem receiverItem, VerificationCode submittedCode)
        {
            if (Status != TransferStatus.Pending)
                throw new InvalidOperationException("Transfer is not in a pending state.");

            if (Code != submittedCode)
                throw new InvalidVerificationCodeException();

            senderItem.RemoveAmount(Amount);
            receiverItem.AddAmount(Amount);
            Status = TransferStatus.Completed;
        }
    }
}
