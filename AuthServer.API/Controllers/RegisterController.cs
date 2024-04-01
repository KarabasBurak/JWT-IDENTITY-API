using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : CustomBaseController
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> Registeration(RegisterDto registerDto)
        {
            
            var result=await _registerService.RegisterationAsync(registerDto);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return CreateActionResult(await _registerService.GetUserByNameAsync(HttpContext.User.Identity.Name));
        }



    }
}
