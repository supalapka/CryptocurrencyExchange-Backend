using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CryptocurrencyExchange.Application.StakingPromotion
{
    public class StakingPromotionService : IStakingPromotionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IOptions<StakingPromotionOptions> _options;
        private readonly ILogger<StakingPromotionService> _logger;

        public StakingPromotionService(
            IUserRepository userRepository,
            IPublishEndpoint publishEndpoint,
            IOptions<StakingPromotionOptions> options,
            ILogger<StakingPromotionService> logger)
        {
            _userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
            _options = options;
            _logger = logger;
        }

        public async Task SendPromotionEmailsAsync()
        {
            var minimumBalance = _options.Value.MinimumUsdtBalance;

            var emails = await _userRepository.GetEmailsForStakingPromotionAsync(minimumBalance);

            _logger.LogInformation(
                "Sending staking promotion emails to {Count} eligible users", emails.Count);

            foreach (var email in emails)
            {
                await _publishEndpoint.Publish(new SendStakingPromotionEmailCommand(email));
            }
        }
    }
}
