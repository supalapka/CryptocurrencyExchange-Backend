using CryptocurrencyExchange.Application.Transfers;
using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Authorize]
    public class TransferController : ApiControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost("auth/transfer/initiate")]
        public async Task<ActionResult<int>> Initiate([FromBody] InitiateTransferDto dto)
        {
            var transferId = await _transferService.InitiateAsync(UserId, dto);

            return Ok(transferId);
        }

        [HttpPost("auth/transfer/confirm")]
        public async Task<ActionResult> Confirm([FromBody] ConfirmTransferDto dto)
        {
            await _transferService.ConfirmAsync(UserId, dto);

            return Ok();
        }
    }
}
