using CryptocurrencyExchange.Application.StakingServices;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class StakingServiceTests
    {
        private Mock<IStakingRepository> _stakingRepoMock;
        private Mock<IWalletItemRepository> _walletRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IStakingDomainService> _domainServiceMock;

        [SetUp]
        public void SetUp()
        {
            _stakingRepoMock = new Mock<IStakingRepository>();
            _walletRepoMock = new Mock<IWalletItemRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _domainServiceMock = new Mock<IStakingDomainService>();

            _unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());
        }

        [Test]
        public async Task CheckForExpiredStakings_UsesExpiredActiveQuery_NotFullScan()
        {
            _stakingRepoMock
                .Setup(x => x.GetExpiredActiveStakingsAsync())
                .ReturnsAsync(new List<Staking>());

            var service = new StakingService(
                _stakingRepoMock.Object,
                _walletRepoMock.Object,
                _unitOfWorkMock.Object,
                _domainServiceMock.Object,
                NullLogger<StakingService>.Instance);

            await service.CheckForExpiredStakings();

            _stakingRepoMock.Verify(x => x.GetExpiredActiveStakingsAsync(), Times.Once);
            _stakingRepoMock.Verify(x => x.GetAllActiveStakingsAsync(), Times.Never);
        }
    }
}
