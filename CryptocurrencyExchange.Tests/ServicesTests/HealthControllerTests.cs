using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class HealthControllerTests
    {
        private Mock<IHealthService> _serviceMock;
        private HealthController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IHealthService>();
            _controller = new HealthController(_serviceMock.Object);
        }

        [Test]
        public void Ping_ReturnsPongPlainText()
        {
            _serviceMock.Setup(x => x.Ping()).Returns("PONG");

            var result = _controller.Ping();

            var contentResult = (ContentResult)result;
            Assert.That(contentResult.Content, Is.EqualTo("PONG"));
            Assert.That(contentResult.ContentType, Is.EqualTo("text/plain"));
        }

        [Test]
        public async Task Latency_ReturnsOkWithLatencyResult()
        {
            var latencyResult = new LatencyResult(42);
            _serviceMock.Setup(x => x.MeasureLatencyAsync(CancellationToken.None)).ReturnsAsync(latencyResult);

            var result = await _controller.Latency(CancellationToken.None);

            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(latencyResult));
        }

        [Test]
        public async Task Status_WhenHealthy_Returns200()
        {
            var dbResult = new DatabaseHealthResult(true, 10, null);
            var statusResult = new HealthStatusResult("Healthy", dbResult, 5, DateTime.UtcNow);
            _serviceMock.Setup(x => x.GetStatusAsync(CancellationToken.None)).ReturnsAsync(statusResult);

            var result = await _controller.Status(CancellationToken.None);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Status_WhenUnhealthy_Returns503()
        {
            var dbResult = new DatabaseHealthResult(false, 10, "error");
            var statusResult = new HealthStatusResult("Unhealthy", dbResult, 5, DateTime.UtcNow);
            _serviceMock.Setup(x => x.GetStatusAsync(CancellationToken.None)).ReturnsAsync(statusResult);

            var result = await _controller.Status(CancellationToken.None);

            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public async Task Database_WhenReachable_Returns200()
        {
            var dbResult = new DatabaseHealthResult(true, 10, null);
            _serviceMock.Setup(x => x.CheckDatabaseAsync(CancellationToken.None)).ReturnsAsync(dbResult);

            var result = await _controller.Database(CancellationToken.None);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Database_WhenUnreachable_Returns503()
        {
            var dbResult = new DatabaseHealthResult(false, 10, "connection refused");
            _serviceMock.Setup(x => x.CheckDatabaseAsync(CancellationToken.None)).ReturnsAsync(dbResult);

            var result = await _controller.Database(CancellationToken.None);

            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(503));
        }
    }
}
