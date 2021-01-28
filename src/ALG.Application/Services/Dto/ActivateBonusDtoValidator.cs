using FluentValidation;

namespace ALG.Application.Services.Dto
{
    public class ActivateBonusDtoValidator : AbstractValidator<ActivateBonusDto>
    {
        public ActivateBonusDtoValidator()
        {
            RuleFor(x => x.ServiceId).NotEmpty();
            RuleFor(x => x.Promocode).NotEmpty();
        }
    }
}
