namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IDatabaseHealthChecker
    {
        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
