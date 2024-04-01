using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStock()
        {
            // Veritabanından, Stock bilgisini getirirken Uniqe değerlere göre (Id ve UserName) stok bilgilerini getirebiliriz. 
            var userName = HttpContext.User.Identity.Name; // Name kısmını, TokenService'de UserName olarak tanımladık

            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock Settings => UserName:{userName}-UserId{userIdClaim.Value}");
        }

    }
}
