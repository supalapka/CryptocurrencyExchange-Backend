using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthDomainService _authDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserRegistrationOutboxRepository _outboxRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuthDomainService authDomainService,
            ITokenService tokenService,
            IUserRegistrationOutboxRepository outboxRepository,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _authDomainService = authDomainService;
            _tokenService = tokenService;
            _outboxRepository = outboxRepository;
            _logger = logger;
        }

        public async Task<string> LoginAsync(Email email, Password password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for email {Email}", email.Value);
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

        public async Task RegisterAsync(Email email, Password password)
        {
            if (await _userRepository.UserExists(email))
            {
                _logger.LogWarning("Registration failed: email {Email} already exists", email.Value);
                throw new UserAlreadyExistsException();
            }

            User user = null;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user = _authDomainService.CreateUser(email, password);
                await _userRepository.AddUserAsync(user);
                await _unitOfWork.CommitAsync();
                await _outboxRepository.AddAsync(new UserRegistrationOutbox(user.Id, user.Email));
            });

            _logger.LogInformation("New user registered with email {Email}", email.Value);
        }
    }
}
