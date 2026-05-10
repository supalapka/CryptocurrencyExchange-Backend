using CryptocurrencyExchange.Application.ApiKeys;
using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Authorize]
    [Route("api/apikeys")]
    public class ApiKeyController : ApiControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<string>> Generate()
        {
            var rawKey = await _apiKeyService.GenerateAsync(UserId);
            return Ok(rawKey);
        }

        [HttpGet]
        public async Task<ActionResult<ApiKeyDto>> Get()
        {
            var dto = await _apiKeyService.GetAsync(UserId);
            return Ok(dto);
        }
    }
}
