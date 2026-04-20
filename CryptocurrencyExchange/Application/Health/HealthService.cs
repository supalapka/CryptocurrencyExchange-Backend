using System.Diagnostics;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;

namespace CryptocurrencyExchange.Application.Health
{
    public class HealthService : IHealthService
    {
        private readonly IDatabaseHealthChecker _databaseHealthChecker;

        public HealthService(IDatabaseHealthChecker databaseHealthChecker)
        {
            _databaseHealthChecker = databaseHealthChecker;
        }

        public string Ping() => "PONG";

        public async Task<LatencyResult> MeasureLatencyAsync(CancellationToken ct = default)
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Yield();
            stopwatch.Stop();
            return new LatencyResult(stopwatch.ElapsedMilliseconds);
        }

        public async Task<DatabaseHealthResult> CheckDatabaseAsync(CancellationToken ct = default)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var isReachable = await _databaseHealthChecker.CanConnectAsync(ct);
                stopwatch.Stop();
                return new DatabaseHealthResult(isReachable, stopwatch.ElapsedMilliseconds, null);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new DatabaseHealthResult(false, stopwatch.ElapsedMilliseconds, ex.Message);
            }
        }

        public async Task<HealthStatusResult> GetStatusAsync(CancellationToken ct = default)
        {
            var db = await CheckDatabaseAsync(ct);
            var latency = await MeasureLatencyAsync(ct);
            var status = db.IsReachable ? "Healthy" : "Unhealthy";
            return new HealthStatusResult(status, db, latency.ElapsedMilliseconds, DateTime.UtcNow);
        }
    }
}
