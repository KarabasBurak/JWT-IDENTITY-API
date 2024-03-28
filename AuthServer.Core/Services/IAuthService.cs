using AuthServer.Core.DTOs;
using SharedLibrary;
using SharedLibrary.Dtos;

namespace AuthServer.Core.Services
{
    public interface IAuthService
    {
        // CreateTokenAsync metoduna veriler LoginDto sınıfındaki proplar ile taşınacak. Taşınan verilerin doğruluğu kontrol edilecek
        // Üretilen token Asenkron olacak şekilde TokenDto'daki proplar ile veriler alınıp Response sınıfındaki Data prop'si ile dönüş olacak. 
        Task<Response<TokenDto>> CreateTokenAsync (LoginDto loginDto); 

        Task<Response<TokenDto>> CreateRefreshToken(string refreshToken);

        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken); // LogOut olduğunda RefreshToken'ı silmek için bu metodu tanımladık

        Response<ClientTokenDto> CreateTokenClient (ClientLoginDto clientLoginDto); // Üyelik sistemi olamadan Client bir API'ye istek atabilir bunun için CreateTokenClient metodu                                                                                           tanımladık. ClientTokenDto ile taşınan ClientId, ClientSecret (ClientId ve ClientSecret                                                                                                 appSettings.json'da tutacağız.) appSettings.json'daki veriler doğru ise  CreateTokenClient                                                                                              metodu ile token üretilecek. Burada RefreshToken olmayacak
    }
}

/*
 Kullanıcının Login, Register olma işlemlerinde kullanacağımız metodları tanımladık. Bu metodlarda token üretilecek
 Kimlik doğrulama burada gerçekleşecek
 */
