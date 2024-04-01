using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.Service.Mapping;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Identity;
using SharedLibrary;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public RegisterService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Response<AppUserDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return Response<AppUserDto>.Fail("UserName not found", 404, true);
            }

            var userDto= _mapper.Map<AppUserDto>(user);

            return Response<AppUserDto>.Success(userDto, 200);
        }

        // Yeni kullanıcı oluşturmak için RegistirationAsync metodunda kodlama yapıyoruz.
        public async Task<Response<AppUserDto>> RegisterationAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return Response<AppUserDto>.Fail("Password and Confirm Password Don't Same", 400, true);
            };

            var user = new AppUser() // registerDto ile taşınan verileri AppUser tablosuna aktarıyorum.
            {
                UserName= registerDto.UserName,
                Email= registerDto.Email,
                
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password); // result nesnesi,IdentityResult sınıfında tanımlıdır.CreateAsync başarılı mı başarısız mı bu durumu                                                                                  tutar

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400);
            }

            var appUserDto= _mapper.Map<AppUserDto>(user); // AppUser tablosuna kullanıcıdan aldığım verileri kaydetmek için AppUser'ı (result), AppUserDto türüne çeviriyorum. 

            return Response<AppUserDto>.Success(appUserDto, 200);

        }
    }
}
