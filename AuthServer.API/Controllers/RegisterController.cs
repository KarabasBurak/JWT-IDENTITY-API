using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : CustomBaseController
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterationAsync(RegisterDto registerDto)
        {
            
            var result=await _registerService.RegisterationAsync(registerDto);
            return CreateActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetUsernameAsync(string username)
        {
            var result = await _registerService.GetUserByNameAsync(username);
            return CreateActionResult(result);
        }



    }
}
