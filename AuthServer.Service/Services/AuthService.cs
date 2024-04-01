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

namespace AuthServer.Service.Services
{
    public class AuthService : IAuthService
    {

        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshToken;

        public AuthService(IOptions<List<Client>> optionClients, ITokenService tokenService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshToken)
        {
            _clients = optionClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshToken = userRefreshToken;
        }

        public async Task<Response<TokenDto>> CreateRefreshToken(string refreshToken)
        {
            var refreshTokenDto = await _userRefreshToken.Where(x=>x.RefreshToken == refreshToken).SingleOrDefaultAsync(); // DB'de RefreshToken var mı ?
            if (refreshTokenDto == null)
            {
                return Response<TokenDto>.Fail("RefreshToken not found", 404, true);
            }

            var user=await _userManager.FindByIdAsync(refreshTokenDto.UserId); // AppUser içinde Id'ye göre arama yap ve refreshTokenDto içindeki UserId'nin aynısını bul ve o Id'ye sahip                                                                          kullanıcı user nesnesine ata.

            if(user == null)
            {
                return Response<TokenDto>.Fail("User Not Found",404,true);
            }

            var tokenDto=_tokenService.CreateToken(user); // Yukarıdan çektiğin user nesnesindeki kullanıcı için token oluştur

            refreshTokenDto.RefreshToken=tokenDto.RefreshToken; 
            refreshTokenDto.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);

        }

        // Login olurken ilgili bilgileri kontrol edip bilgiler doğru ise üretilecek token işlemlerini yapıyoruz
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }
            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshToken.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshToken.AddAsync(new UserRefreshToken { UserId = user.Id, RefreshToken = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.RefreshToken = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);
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

        // Kullanıcı LogOut olursa RefreshToken'ı silme işlemini RevokeRefreshToken metodu ile gerçekleştireceğiz. 
        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existResfreshToken = await _userRefreshToken.Where(x => x.RefreshToken == refreshToken).SingleOrDefaultAsync();

            if(existResfreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh Token Not Found",404,true);
            }

            _userRefreshToken.Remove(existResfreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);


        }
    }
}
