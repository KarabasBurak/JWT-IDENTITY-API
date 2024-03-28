using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthService : IAuthService
    {

        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthService(IOptions<List<Client>> optionClients, ITokenService tokenService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public Task<Response<TokenDto>> CreateRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        // Login olurken üretilecek token işlemlerini yapıyoruz
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if(loginDto == null) // Client tarafından login bilgilerini(email ve password) alıyoruz ve boş/dolu olduğunu kontrol ediyoruz.
            {
                throw new ArgumentException(nameof(loginDto));
            }

            var user= await _userManager.FindByEmailAsync(loginDto.Email);  // loginDto dolu ise loginDto'dan gelen Emaile göre veritabanında bu email var mı/yok mu kontrol ediyoruz.

            if (user==null)
            {
                return Response<TokenDto>.Fail("Email or Password is Wrong",400,true);
            }

            var password= await _userManager.CheckPasswordAsync(user,loginDto.Password);  // Email var ise password doğruluğu kontrol (true veya false ile) ediyoruz.
            if (!password)
            {
                return Response<TokenDto>.Fail("Email or Password is Wrong", 400, true);
            }

            var token = _tokenService.CreateToken(user); // ITokenService'deki CreateToken metodunu çağırdık ve token oluştururken AppUser için oluşturacağımız için user değişkenini verdik

            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync(); // Önce refresh token var mı yok mu kontrol ettik. yok ise token                                                                                                                          ürettik

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken {UserId=user.Id, RefreshToken=token.RefreshToken, Expiration=token.RefreshTokenExpiration});
            }
            else
            {
                userRefreshToken.RefreshToken = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token,200);

        }

        // Client, üyelik sistemi gerektirmeyen API'lere istek atarken token gereklidir. CreateTokenClient metodu ile token üretilecek
        public Response<ClientTokenDto> CreateTokenClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

            if(client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404,true);
            }

            var token= _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);

        }

        public Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
