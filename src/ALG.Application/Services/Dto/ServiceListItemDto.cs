using System;

namespace ALG.Application.Services.Dto
{
    public class ServiceListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Activated { get; set; }
    }
}
