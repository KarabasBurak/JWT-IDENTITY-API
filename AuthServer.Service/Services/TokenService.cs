using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CustomTokenDto _customTokenDto;

        public TokenService(UserManager<AppUser> userManager, IOptions<CustomTokenDto>options)
        {
            _userManager = userManager;
            _customTokenDto = options.Value;
        }

        // RefreshToken oluşturmak için private türünde metod oluşturduk.
        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];

            using var randomNumber=RandomNumberGenerator.Create(); // Random bir değer ürettik

            randomNumber.GetBytes(numberByte); // Yukarıda numberByte değişkenine atanan Byte'ları randomNumber değerine aktardı

            return Convert.ToBase64String(numberByte);

            // return Guid.NewGuid().ToString(); Guid üzerinden de refresh token üretilir.
        }


        // Üyelik sistemi gerektiren durumlarda GetClaimsUser metodunu kullanacağız
        private IEnumerable<Claim> GetClaimsUser(AppUser appUser,List<String> audiences) 
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,appUser.Id), //AppUser tablosundan Id'yi aldık ClaimTypes.NameIdentifier'a atadık. Atadığımız Claimler ile bu verileri taşıyacağız
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim("City",appUser.City),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userList.AddRange(audiences.Select(x=>new Claim(JwtRegisteredClaimNames.Aud,x)));

            return userList;


            /* BUNLAR KULLANICI İLE İLGİLİ CLAİMLER (AppUser'dan veri çekeceğiz)
             Bu kod parçası, appUser nesnesinden alınan kullanıcı bilgilerini kullanarak bir Claim listesi oluşturur ve bu listeye kullanıcıya özel claim'leri ekler. Bu liste daha sonra, token oluşturma, kimlik doğrulama veya yetkilendirme gibi işlemlerde kullanılabilir. Bu sayede, sistemin farklı bölümleri, kullanıcının kimliği ve yetkileri hakkında bilgi sahibi olabilir. 
             */
        }

        // Üyelik sistemi gerektirmeyen durumlarda GetClaimsUser metodunu kullanacağız
        private IEnumerable<Claim> GetClaimsClient(Client client)
        {
            var claimsClient = new List<Claim>();

            claimsClient.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub,client.Id.ToString());

            return claimsClient;
        }

        // Token Oluşturma metodu
        public TokenDto CreateToken(AppUser appUser)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenDto.AccessTokenExpiration); // 5 dk gelirse var olan saatte 5 dk ekleyecek
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenDto.RefreshTokenExpiration);
            var securityKey=SignService.GetSymmetricSecurityKey(_customTokenDto.SecurityKey); // Token'ı imzalayacak Key 
            SigningCredentials signingCredentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature); // Token İmza kısmı

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _customTokenDto.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsUser(appUser,_customTokenDto.Audience),
                signingCredentials: signingCredentials
                );

            var handler=new JwtSecurityTokenHandler(); // Token'ı JwtSecurityTokenHandler oluşturacak.
            var token=handler.WriteToken(jwtSecurityToken); // WriteToken; issuer, expires,notBefore,claims,signingCredentials bilgilere göre string bir token ürettiyor.
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            {
                var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenDto.AccessTokenExpiration); // 5 dk gelirse var olan saatte 5 dk ekleyecek
                
                var securityKey = SignService.GetSymmetricSecurityKey(_customTokenDto.SecurityKey); // Token'ı imzalayacak Key 
                SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature); // Token İmza kısmı

                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                    (
                    issuer: _customTokenDto.Issuer,
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: GetClaimsClient(client),
                    signingCredentials: signingCredentials
                    );

                var handler = new JwtSecurityTokenHandler(); // Token'ı JwtSecurityTokenHandler oluşturacak.
                var token = handler.WriteToken(jwtSecurityToken); // WriteToken; issuer, expires,notBefore,claims,signingCredentials bilgilere göre string bir token ürettiyor.
                var clientTokenDto = new ClientTokenDto   //
                {
                    AccessToken = token,
                    AccessTokenExpiration = accessTokenExpiration,
                    
                };
                return clientTokenDto;
            }
        }
    }
}
