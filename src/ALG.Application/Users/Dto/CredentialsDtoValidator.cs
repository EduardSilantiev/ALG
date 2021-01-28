using FluentValidation;

namespace ALG.Application.Users.Dto
{
    public class CredentialsDtoValidator : AbstractValidator<CredentialsDto>
    {
        public CredentialsDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
