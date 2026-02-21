namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface ICryptoNewsUpdateRequester
    {
        Task TriggerUpdate(string coin);
    }
}
