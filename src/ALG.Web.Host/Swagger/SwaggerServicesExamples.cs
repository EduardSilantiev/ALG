using ALG.Application.Helpers.Exceptions;
using ALG.Application.Helpers.Paging;
using ALG.Application.Services.Dto;
using ALG.Core.Services;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Seed;
using AutoMapper;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace ALG.Web.Host.Swagger
{
    public class ServiceListDtoResponseExample : IExamplesProvider<object>
    {
        private readonly IMapper _mapper;

        public ServiceListDtoResponseExample(IMapper mapper)
        {
            _mapper = mapper;
        }

        public object GetExamples()
        {
            var services = InitDefaults.GetInitialServices();
            var servicePagingList = new PaginatedList<Service>(services, 1, 5);
            var serviceListDto = new ServiceListDto(servicePagingList, _mapper);
            return serviceListDto;
        }
    }

    public class ActivateBonusRequestExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ActivateBonusDto()
            {
                ServiceId = Guid.NewGuid(),
                Promocode = "itpromocode"
            };
    }

    public class ActivateBonusBadRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("Service ID is incorrect.");
    }

    public class ActivateBonusConflictRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("Bonus for this service is already activated.");
    }

    public class GetListOfServicesUnprocessableEntityResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("'Current Page' must be greater than or equal to '0'.");
    }


    public class ActivateUnprocessableEntityResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("'Promocode' must not be empty.");
    }


}
