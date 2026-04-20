using CryptocurrencyExchange.Application.Health;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class HealthServiceTests
    {
        private Mock<IDatabaseHealthChecker> _checkerMock;
        private HealthService _service;

        [SetUp]
        public void SetUp()
        {
            _checkerMock = new Mock<IDatabaseHealthChecker>();
            _service = new HealthService(_checkerMock.Object);
        }

        [Test]
        public void Ping_ReturnsPong()
        {
            Assert.That(_service.Ping(), Is.EqualTo("PONG"));
        }

        [Test]
        public async Task MeasureLatencyAsync_ReturnsNonNegativeElapsedMs()
        {
            var result = await _service.MeasureLatencyAsync();

            Assert.That(result.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task CheckDatabaseAsync_WhenCheckerReturnsTrue_ReturnsReachableTrueAndNullError()
        {
            _checkerMock.Setup(x => x.CanConnectAsync(default)).ReturnsAsync(true);

            var result = await _service.CheckDatabaseAsync();

            Assert.That(result.IsReachable, Is.True);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task CheckDatabaseAsync_WhenCheckerThrows_ReturnsReachableFalseWithErrorMessage()
        {
            _checkerMock.Setup(x => x.CanConnectAsync(default))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var result = await _service.CheckDatabaseAsync();

            Assert.That(result.IsReachable, Is.False);
            Assert.That(result.Error, Does.Contain("boom"));
        }

        [Test]
        public async Task GetStatusAsync_WhenDbReachable_ReturnsHealthy()
        {
            _checkerMock.Setup(x => x.CanConnectAsync(default)).ReturnsAsync(true);

            var result = await _service.GetStatusAsync();

            Assert.That(result.Status, Is.EqualTo("Healthy"));
        }

        [Test]
        public async Task GetStatusAsync_WhenDbUnreachable_ReturnsUnhealthy()
        {
            _checkerMock.Setup(x => x.CanConnectAsync(default)).ReturnsAsync(false);

            var result = await _service.GetStatusAsync();

            Assert.That(result.Status, Is.EqualTo("Unhealthy"));
        }
    }
}
