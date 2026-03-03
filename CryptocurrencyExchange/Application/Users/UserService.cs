using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GetEmailByIdAsync(int userId)
        {
            string email = await _userRepository.GetEmailByIdAsync(userId)
                ?? throw new UserNotFoundException();

            return email;
        }
    }
}
