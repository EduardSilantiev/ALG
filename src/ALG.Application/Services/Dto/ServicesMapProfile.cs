using ALG.Core.Services;
using AutoMapper;
using System.Linq;

namespace ALG.Application.Services.Dto
{
    public class ServicesMapProfile : Profile
    {
        public ServicesMapProfile()
        {
            CreateMap<Service, ServiceListItemDto>()
                .ForMember(dest => dest.Activated, opt =>
                    opt.MapFrom((src, dest) =>
                    {
                        return src.ActivatedBonuses.Any();
                    }));
        }
    }
}
