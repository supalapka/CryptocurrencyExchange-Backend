using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Services.Futures
{
    public class FuturesService : IFuturesService
    {
        private readonly IFuturesDomainService _futuresDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IFutureRepository _futureRepository;

        private const string UsdtSymbol = "usdt";


        public FuturesService(
            IFuturesDomainService futuresDomainService,
            IUnitOfWork unitOfWork,
            IWalletItemRepository walletItemRepository,
            IFutureRepository futureRepository
            )
        {
            _futuresDomainService = futuresDomainService;
            _unitOfWork = unitOfWork;
            _walletItemRepository = walletItemRepository;
            _futureRepository = futureRepository;
        }


        public async Task<int> CreateFutureAsync(FutureDto futureDto, int userId)
        {
            Future future = null!;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var userUsdt = await _walletItemRepository.GetAsync(userId, UsdtSymbol)
                    ?? throw new WalletItemNotFoundException(UsdtSymbol);

                future = _futuresDomainService.OpenPosition(futureDto, userUsdt);

                await _futureRepository.AddAsync(future);
            });

            return future.Id;
        }



        public async Task<List<FutureDto>> GetFuturePositions(int userId)
        {
            var positions = await _futureRepository.GetCompletedFuturesAsync(userId);

            return positions
               .OrderByDescending(x => x.Id)
               .Select(x => new FutureDto
               {
                   Id = x.Id,
                   Symbol = x.Symbol,
                   EntryPrice = x.EntryPrice,
                   Margin = x.Margin,
                   Leverage = x.Leverage,
                   Position = x.Position
               })
               .ToList();
        }


        public async Task LiquidatePosition(int id, double markPrice)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var position = await _futureRepository.GetByIdAsync(id)
                    ?? throw new Exception($"Futures position not found. ID: {id}");

                _futuresDomainService.LiquidatePosition(position);

                await _futureRepository.AddPositionToHistoryAsync(position, markPrice);
            });
        }


        public async Task ClosePosition(int id, decimal pnl, double markPrice)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var position = await _futureRepository.GetByIdAsync(id)
                 ?? throw new Exception($"Futures position not found. ID: {id}");

                var userUsdt = await _walletItemRepository.GetAsync(position.UserId, "usdt")
                 ?? throw new WalletItemNotFoundException(UsdtSymbol);

                _futuresDomainService.ClosePosition(position, userUsdt, pnl);

                await _futureRepository.AddPositionToHistoryAsync(position, markPrice);
            });
        }


        public async Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page)
        {
            int positionsPerPage = 5;
            return await _futureRepository.GetHistoryAsync(userId, page, positionsPerPage);
        }
    }
}
