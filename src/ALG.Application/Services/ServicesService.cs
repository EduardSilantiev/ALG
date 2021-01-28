using ALG.Application.Application.Settings;
using ALG.Application.Helpers;
using ALG.Application.Helpers.Paging;
using ALG.Application.Services.Dto;
using ALG.Core.Services;
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.Application.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IServicesRepository _servicesRepository;
        private readonly IMapper _mapper;
        private readonly PagingSettings _settings;

        public ServicesService(IServicesRepository servicesRepository, IMapper mapper, IOptions<PagingSettings>  settings)
        {
            _servicesRepository = servicesRepository;
            _mapper = mapper;
            _settings = settings.Value;
        }

        /// <summary>
        /// Obtaines a filtered and paginated list of services
        /// </summary>
        /// <param name="userId">User id (to define activated services)</param>
        /// <param name="getServicesListDto">Model for getting the list with filtering and paging</param>
        /// <returns>Returns the filtered and paginated list of services</returns>
        public async Task<ServiceListDto> GetAllServicesAsync(Guid userId, GetServicesListDto getServicesListDto)
        {
            var services = await PaginatedList<Service>.FromIQueryable(_servicesRepository.GetAllServicesAsync(userId,
                                                                        getServicesListDto.Filter),
                        getServicesListDto.CurrentPage,
                        getServicesListDto.PageSize == 0 ? _settings.PageSize : getServicesListDto.PageSize);

            var serviceListDto = new ServiceListDto(services, _mapper);
            return serviceListDto;
        }

        /// <summary>
        /// Verifies that a bonus can be activated for a user and a service
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="activateBonusDto">Model for activating the bonus with a Service Id and a Promocode</param>
        /// <returns>A CanBeProcessed result</returns>
        public async Task<CanBeProcessedDto> BonusCanBeActivatedAsync(Guid userId, ActivateBonusDto activateBonusDto)
        {
            var service = await _servicesRepository.GetServiceAsync(userId, activateBonusDto.ServiceId);
            if (service == null)
                return new CanBeProcessedDto
                {
                    CanBeProcessed = false,
                    StatusCode = (int)AppConsts.HttpStatusCodes.Status400BadRequest,
                    RejectionReason = $"Service is not found by id={activateBonusDto.ServiceId}."
                };

            if (service.Promocode != activateBonusDto.Promocode)
                return new CanBeProcessedDto
                {
                    CanBeProcessed = false,
                    StatusCode = (int)AppConsts.HttpStatusCodes.Status400BadRequest,
                    RejectionReason = $"Promocode '{activateBonusDto.Promocode}' is not valid for the service."
                };


            if (service.ActivatedBonuses.Any())
                return new CanBeProcessedDto
                {
                    CanBeProcessed = false,
                    StatusCode = (int)AppConsts.HttpStatusCodes.Status409Conflict,
                    RejectionReason = $"Bonus is already activated for the service."
                };

            return new CanBeProcessedDto
            {
                CanBeProcessed = true,
                StatusCode = (int)AppConsts.HttpStatusCodes.Status204NoContent,
                RejectionReason = string.Empty
            };
        }

        /// <summary>
        /// Activates a bonus for a user and a service
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="activateBonusDto">Model for activating the bonus with a Service Id and a Promocode</param>
        /// <returns></returns>
        public async Task ActivateBonusAsync(Guid userId, ActivateBonusDto activateBonusDto)
        {
            var activatedBonus = new ActivatedBonus
            {
                UserId = userId,
                ServiceId = activateBonusDto.ServiceId
            };

            await _servicesRepository.ActivateBonusAsync(activatedBonus);
        }
    }
}
