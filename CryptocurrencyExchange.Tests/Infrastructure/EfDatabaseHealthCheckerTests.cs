using CryptocurrencyExchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Infrastructure
{
    [TestFixture]
    public class EfDatabaseHealthCheckerTests
    {
        private DataContext _context;
        private EfDatabaseHealthChecker _checker;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new DataContext(options);
            _checker = new EfDatabaseHealthChecker(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CanConnectAsync_WhenDatabaseAvailable_ReturnsTrue()
        {
            var result = await _checker.CanConnectAsync();

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanConnectAsync_AfterContextDisposed_ThrowsOrReturnsFalse()
        {
            _context.Dispose();

            try
            {
                var result = await _checker.CanConnectAsync();
                Assert.That(result, Is.False);
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.InstanceOf<ObjectDisposedException>().Or.InstanceOf<InvalidOperationException>());
            }
        }
    }
}
