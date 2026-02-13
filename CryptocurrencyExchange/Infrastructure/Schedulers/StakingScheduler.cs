using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Services.StakingServices;

namespace CryptocurrencyExchange.Infrastructure.Schedulers
{
    public class StakingScheduler : BackgroundService
    {
        private Timer timer;

        private readonly IServiceProvider _serviceProvider;

        public StakingScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var stakingService = scope.ServiceProvider.GetRequiredService<IStakingService>();
                stakingService.CheckForExpiredStakings();
            }

            DateTime startTime = DateTime.Today.AddDays(1);
            TimeSpan timeUntilStart = startTime - DateTime.Now;

            timer = new Timer(CheckStaking, null, timeUntilStart, TimeSpan.FromDays(1));



            //for testing
            //DateTime currentTime = DateTime.Now;
            //DateTime desiredTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 15, 0);

            //if (currentTime > desiredTime)
            //    desiredTime = desiredTime.AddDays(1);

            //TimeSpan timeToWait = desiredTime - currentTime;
            //Timer timer = new Timer(CheckStaking, null, timeToWait, TimeSpan.FromMilliseconds(-1));
        }

        private void CheckStaking(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var stakingService = scope.ServiceProvider.GetRequiredService<IStakingService>();
                stakingService.CheckForExpiredStakings();
            }
        }
    }
}
