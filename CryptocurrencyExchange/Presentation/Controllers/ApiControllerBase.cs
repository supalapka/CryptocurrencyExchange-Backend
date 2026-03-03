using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int UserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
