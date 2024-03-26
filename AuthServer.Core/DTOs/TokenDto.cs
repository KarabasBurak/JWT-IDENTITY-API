using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class TokenDto
    {
        
        public string? AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}

/*
 1) Client, AccessToken ve RefreshToken alabilmek için AuthServer'a Username ve Password bilgilerini gönderecek.
 2) AuthServer username ve password bilgilerini kontroledecek doğru ise Client'a AccessToken ve RefreshToken dönecek.
 3) Client, aldığı AccessToken ile API'ye istek atabilecek
 */



