using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletItemRepository _walletRepository;
        private readonly IAuthDomainService _authDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository
            userRepository,
            IUnitOfWork unitOfWork,
            IWalletItemRepository walletRepository,
            IAuthDomainService authDomainService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _walletRepository = walletRepository;
            _authDomainService = authDomainService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for email {Email}", email);
                throw new UserNotFoundException();
            }

            if (!_authDomainService.VerifyPassword(password, user))
            {
                _logger.LogWarning("Login failed: wrong password for user {UserId}", user.Id);
                throw new InvalidPasswordException();
            }

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return _tokenService.CreateToken(user);
        }

        public async Task RegisterAsync(string email, string password)
        {
            if (await _userRepository.UserExists(email))
            {
                _logger.LogWarning("Registration failed: email {Email} already exists", email);
                throw new UserAlreadyExistsException();
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                User user = _authDomainService.CreateUser(email, password);

                await _userRepository.AddUserAsync(user);

                await CreateStarterWalletAsync(user);
            });

            _logger.LogInformation("New user registered with email {Email}", email);
        }

        public async Task<string> GetEmailByIdAsync(int userId)
        {
            string email = await _userRepository.GetEmailByIdAsync(userId)
                ?? throw new UserNotFoundException();

            return email;
        }

        private Task CreateStarterWalletAsync(User user)
        {
            var walletItem = new WalletItem(user, new CoinSymbol(CoinSymbol.Usdt.Value));
            walletItem.AddAmount(5000);

            return _walletRepository.AddAsync(walletItem);
        }
    }
}
