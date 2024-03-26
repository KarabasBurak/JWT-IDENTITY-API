﻿using AuthServer.Core.DTOs;
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
        Task<Response<AppUserDto>> RegistirationAsync(RegisterDto registerDto);

        Task<Response<AppUserDto>> GetUsernameAsync(string username); // DB'den username nesnesi kullanıcı çekip dönmek için bu metod tanımlandı
    }
}