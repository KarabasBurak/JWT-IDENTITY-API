using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(AppUser appUser); // Geriye TokenDto sınıfındaki propertyler ile dönüş yapılacak. CreateToken metodu AppUser için Token oluşturma metodudur.

        ClientTokenDto CreateTokenByClient(Client client); // ClietnTokenDto sınıfında tanımlanan propertyler veri taşınıp dönülecek. Client'ın API'ye atacağı isteğin metodu tanımlandı
    }
}

/*
 ITokenService interfacesinde Response dönmeyeceğiz. Response, IAuth interfacesinde döneceğiz
 
 */
