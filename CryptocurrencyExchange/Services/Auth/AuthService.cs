using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Infrastructure.Security;
using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletItemRepository _walletRepository;
        private readonly IAuthDomainService _authDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository
            userRepository,
            IUnitOfWork unitOfWork,
            IWalletItemRepository walletRepository,
            IAuthDomainService authDomainService,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _walletRepository = walletRepository;
            _authDomainService = authDomainService;
            _tokenService = tokenService;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new UserNotFoundException();

            if (!_authDomainService.VerifyPassword(password, user))
                throw new Exception("Wrong Password");

            return _tokenService.CreateToken(user);

        }

        public async Task RegisterAsync(string email, string password)
        {
            if (await _userRepository.UserExists(email))
                throw new UserAlreadyExistsException();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                User user = _authDomainService.CreateUser(email, password);

                await _userRepository.AddUserAsync(user);

                await CreateStarterWalletAsync(user);
            });
        }

        public async Task<string> GetEmailByIdAsync(int userId)
        {
            string email = await _userRepository.GetEmailByIdAsync(userId)
                ?? throw new UserNotFoundException();

            return email;
        }

        private Task CreateStarterWalletAsync(User user)
        {
            var walletItem = new WalletItem
            {
                Symbol = "usdt",
                Amount = 5000,
                User = user
            };

            return _walletRepository.AddAsync(walletItem);
        }


    }
}
