using FluentValidation;

namespace ALG.Application.Helpers.Paging.Dto
{
    public class GetPaginatedListDtoValidator : AbstractValidator<GetPaginatedListDto>
    {
        public GetPaginatedListDtoValidator() : base()
        {
            RuleFor(x => x.CurrentPage).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(-1);
        }
    }
}
