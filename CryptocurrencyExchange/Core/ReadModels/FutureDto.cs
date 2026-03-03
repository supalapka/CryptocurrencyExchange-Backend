using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Core.ReadModels
{
    public record FutureDto(
    int Id,
    string Symbol,
    decimal Margin,
    decimal EntryPrice,
    int Leverage,
    PositionType Position
    );
}