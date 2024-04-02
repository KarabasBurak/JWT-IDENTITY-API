using AuthServer.Core.Configuration;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Mapping;
using AuthServer.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Exceptions;
using SharedLibrary.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DI Register (Burasi EF Core ile gelen DI containerdir. AutoFac gibi baska DI Coniner projeye dahil edilebilir.)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // 1 tane generic aldigi için virgül koymadik. yani sadece <T> aldi
builder.Services.AddScoped(typeof(IGenericService<,>),typeof(GenericService<,>)); // <T,TDto> 2 tane generic aldigi için bir virgül <,> koyduk. 3 tane generic aldiysa 2 virgül koyariz
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MapProfile));

// AddIdentity metodu ile uygulamaya kimlik doğrulama ve yetkilendirme özellikleri eklenir.
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true; // Emaili Unique yaptik. Yani ayni emailden birden fazla olamaz.
    opt.Password.RequireNonAlphanumeric = true; // *, ? gibi işretlerin kullanılmasını zorunlu kıldık.
})
    .AddEntityFrameworkStores<AppDbContext>() // Asp.Net Core Identity kullaniciların bilgilerini saklamak icin EF Core'un veritabanini (AppDbContext) kullanacagini bildiriyoruz. 
    .AddDefaultTokenProviders();      // e-posta ile kullanıcı doğrulama, şifre sıfırlama, iki faktörlü kimlik doğrulama (2FA) gibi işlemler için gerekli olan token sağlayıcılarını ekler.



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), x =>
    {
        x.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});


// Configure ile beraber DI nesnesi olarak ekledik. asagidaki siniflara herhangi bir sinifin constructor'indan erisebiliriz. Yani DI üzerinden appSettings içindeki datalara erisme islemine OptionPatterns diyoruz
builder.Services.Configure<CustomTokenDto>(
    builder.Configuration.GetSection("TokenOption")
);
builder.Services.Configure<List<Client>>(
    builder.Configuration.GetSection("Clients")
);



//JWT Authentication yapılandırması. Yani API projemizdeki endpointe token ile istek yapıldığında bu token'nın doğrulama işlemi gerçekleştiriyoruz.

builder.Services
    .AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Kaç tane üyelik sistemi var ise onu ekleriz. Bizde 1 tane olduğu için Default Şemayı kullandık.
    opt.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;

})
    // AddJWTBearer metodu, JWT bazlı kimlik doğrulama yapacağımızı bildirdik. endPointe gelen requestin Token'nındaki header'ı arayacak ve doğrulayacak

    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
    {
        var tokenOptions=builder.Configuration.GetSection("TokenOption").Get<CustomTokenDto>(); // CustomTokenDto'dan tokenOptions nesne örneği oluşturduk
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            // appSettings'de tanımladığımız TokenOptionlar var. TokenOption'lardaki tanımlanan sabitleri, JWT yapısındaki sabitlere tanımladık.
            ValidIssuer = tokenOptions.Issuer, 
            ValidAudience = tokenOptions.Audience[0], // Birden fazla audience var biz dizideki ilk elemanı aldık. Çünkü ana projemiz
            IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

            // JWT sabitlerini appSettings'deki sabitler ile tanımladık. Burada da Client tarafından gelen Token'daki sabitler ile JWT sabitleri aynı olup olmadığını kontrol ediyoruz. Yani JWT'yi Validate ediyoruz. true dememizin sebebi imzayı, audienceyi doğrula anlamındadır.
            ValidateIssuerSigningKey=true,
            ValidateAudience=true,
            ValidateIssuer=true,
            ValidateLifetime=true,
            ClockSkew=TimeSpan.Zero
        };
    });



// FluentValidation Metodları eklendi
builder.Services.AddFluentValidation(option =>
{
    option.RegisterValidatorsFromAssemblyContaining<Program>();
});

builder.Services.UseCustomValidationResponse();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // app.UseCustomException();
}

app.UseCustomException(); // Bu metodu CustomExceptionHandler sınıfında oluşturduk. hataları yakalamak için
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
