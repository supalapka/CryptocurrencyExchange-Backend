namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
