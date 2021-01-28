using ALG.Application.Helpers.Paging;
using ALG.Application.Helpers.Paging.Dto;
using ALG.Core.Services;
using AutoMapper;
using System.Collections.Generic;

namespace ALG.Application.Services.Dto
{
    /// <summary>
    /// A class that provides paging of a collection of Services
    /// </summary>
    public class ServiceListDto : PagingDto
    {
        public ICollection<ServiceListItemDto> Items { get; set; }

        public ServiceListDto(PaginatedList<Service> paginatedList, IMapper mapper) :
                base(paginatedList.CurrentPage, paginatedList.PageSize, paginatedList.TotalItems, paginatedList.TotalPages)
        {
            this.Items = mapper.Map<ICollection<ServiceListItemDto>>(paginatedList);
        }
    }
}
