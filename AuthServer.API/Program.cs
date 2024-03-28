using AuthServer.Core.Configuration;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure ile beraber DI nesnesi olarak ekledik. asagidaki siniflara herhangi bir sinifin constructor'indan erisebiliriz. Yani DI üzerinden appSettings içindeki datalara erisme islemine OptionPatterns diyoruz
builder.Services.Configure<CustomTokenDto>(
    builder.Configuration.GetSection("TokenOption")
);
builder.Services.Configure<List<Client>>(
    builder.Configuration.GetSection("Clients")
);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
