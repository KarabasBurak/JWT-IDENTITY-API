using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController   // Authentication işlemlerini (Kimlik doğrulama, Token oluşturma) bu controllerda yapacağız
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenAsync(LoginDto loginDto)
        {
            var result=await _authService.CreateTokenAsync(loginDto); // CreateTokenAsync metodunu AuthService sınıfında doldurduk. Burada CreateTokenAsync metodunu çağıracağız.
            return CreateActionResult(result);

        }

        [HttpPost]
        public IActionResult CreateTokenClient(ClientLoginDto clientLoginDto)
        {
            var result=_authService.CreateTokenClient(clientLoginDto);
            return CreateActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result= await _authService.RevokeRefreshToken(refreshTokenDto.RefreshToken);
            return CreateActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result= await _authService.CreateRefreshToken(refreshTokenDto.RefreshToken);
            return CreateActionResult(result);
        }


    }
}
