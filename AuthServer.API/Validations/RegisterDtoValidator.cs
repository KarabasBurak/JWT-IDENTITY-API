using AuthServer.Core.DTOs;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is Required").EmailAddress().WithMessage("Email is wrong");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");


        }
    }
}

/*
 Program.cs'de FluentValidation kullanacağını bildir.
 */
  