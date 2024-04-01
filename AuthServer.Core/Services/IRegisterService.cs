using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IRegisterService
    {
        Task<Response<AppUserDto>> RegisterationAsync(RegisterDto registerDto);

        Task<Response<AppUserDto>> GetUserByNameAsync(string userName); // DB'den username nesnesi kullanıcı çekip dönmek için bu metod tanımlandı
    }
}
