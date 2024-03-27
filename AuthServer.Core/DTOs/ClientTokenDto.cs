using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class ClientTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }

    }
}

/*
 ClientTokenDto sadece AccessToken transferi yapılacak. RefreshToken burada yok. AccessToken ile API'ye istek yapabilmesi için ClientTokenDto tanımlandı ve bu Dto üzerinden AccessToken gönderecek API'ye. Ama üyelik işlemi gerektiren API için değil
 */
