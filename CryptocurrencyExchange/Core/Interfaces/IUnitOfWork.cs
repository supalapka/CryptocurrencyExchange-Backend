namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);

    }
}
