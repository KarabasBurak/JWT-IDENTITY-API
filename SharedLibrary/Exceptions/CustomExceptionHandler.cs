using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System.Text.Json;

namespace SharedLibrary.Exceptions
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>   // Uygulamada oluşan hatayı UseExceptionHandler metodu ile yakalarız.
            {
                config.Run(async context =>     // Run metodu son metoddur. Yani Request devam etmez. 
                {
                    context.Response.StatusCode = 500;       // HttpContext sınıfından context nesnesi ürettik. HttpResponse'ın Response'ın StatusCode 500 atadık. Sistemsel hatalar için
                    context.Response.ContentType = "application/json";   // Hatanın tipini belirledik.

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();    // IExceptionHandlerFeature ile hatayı yakala errorFeature nesnesine ata
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;  // errorFeature dolu ise Exception sınıfından gelen Error'u ex'e ata
                        ErrorDto errorDto = null;   // ErrorDto sınıfından boş bir errorDto nesnesi oluşturduk. Hatayı buradaki propertyler ile taşıyıp göstereceğiz.

                        if(ex is CustomException)   
                        {
                            errorDto=new ErrorDto(ex.Message,true); // Yakalanan hatayı ErrorDto ile CustomException ise hata mesajını kullanıcıya göster
                        }
                        else
                        {
                            errorDto=new ErrorDto(ex.Message,false); // Değilse gösterme
                        }

                        var response = Response<NoDataDto>.Fail(errorDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
