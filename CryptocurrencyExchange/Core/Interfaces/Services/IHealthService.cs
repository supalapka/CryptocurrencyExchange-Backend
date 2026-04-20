namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public record LatencyResult(long ElapsedMilliseconds);

    public record DatabaseHealthResult(bool IsReachable, long ElapsedMilliseconds, string? Error);

    public record HealthStatusResult(string Status, DatabaseHealthResult Database, long LatencyMs, DateTime TimestampUtc);

    public interface IHealthService
    {
        string Ping();
        Task<LatencyResult> MeasureLatencyAsync(CancellationToken ct = default);
        Task<HealthStatusResult> GetStatusAsync(CancellationToken ct = default);
        Task<DatabaseHealthResult> CheckDatabaseAsync(CancellationToken ct = default);
    }
}
