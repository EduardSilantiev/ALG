using ALG.Application.Helpers;
using ALG.Application.Services.Dto;
using System;
using System.Threading.Tasks;

namespace ALG.Application.Services
{
    public interface IServicesService
    {
        Task<ServiceListDto> GetAllServicesAsync(Guid userId, GetServicesListDto getServicesListDto);
        Task<CanBeProcessedDto> BonusCanBeActivatedAsync(Guid userId, ActivateBonusDto activateBonusDto);
        Task ActivateBonusAsync(Guid userId, ActivateBonusDto activateBonusDto);
    }
}
