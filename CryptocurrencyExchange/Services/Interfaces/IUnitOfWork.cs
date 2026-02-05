namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);

    }
}
