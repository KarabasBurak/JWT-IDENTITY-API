using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenDto customTokenDto)
        {
            services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Kaç tane üyelik sistemi var ise onu ekleriz. Bizde 1 tane olduğu için Default Şemayı kullandık.
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    // AddJWTBearer metodu, JWT bazlı kimlik doğrulama yapacağımızı bildirdik. endPointe gelen requestin Token'nındaki header'ı arayacak ve doğrulayacak

    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
    {
        
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            // appSettings'de tanımladığımız TokenOptionlar var. TokenOption'lardaki tanımlanan sabitleri, JWT yapısındaki sabitlere tanımladık.
            ValidIssuer = customTokenDto.Issuer,
            ValidAudience = customTokenDto.Audience[0], // Birden fazla audience var biz dizideki ilk elemanı aldık. Çünkü ana projemiz
            IssuerSigningKey = SignService.GetSymmetricSecurityKey(customTokenDto.SecurityKey),

            // JWT sabitlerini appSettings'deki sabitler ile tanımladık. Burada da Client tarafından gelen Token'daki sabitler ile JWT sabitleri aynı olup olmadığını kontrol ediyoruz. Yani JWT'yi Validate ediyoruz. true dememizin sebebi imzayı, audienceyi doğrula anlamındadır.
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
        }
    }
}

/*
 CustomTokenAuth sınıfında AddCustomTokenAuth metodunu oluşturmamızın amacı; Client, MiniApp.API projelerine erişmek için API projesinden token alıyor (Bundan dolayı AddAuthentication metodunu yazdık). MiniApp.API projelerine erişebilmesi için bu projelerde appSettings dosyalarına ayrı olarak TokenOption(audience,ıssuer gibi) tanımlanır ve MiniApp.API projelerinin program.cs'de kullanmak için tüm API projelerinde Authentication kodlamasını ayrı ayrı tanımlamak yerine CustomTokenAuth sınıfında tanımladık ve bu static sınıfı MiniApp.API projelerinin program.cs'de tanımladık. Yani bu bir extension oluşturduk. Extension oluşturmamızın amacı kod tekrarından kaçınmaktır (3 API'de de kullandık) . 
 
 */
