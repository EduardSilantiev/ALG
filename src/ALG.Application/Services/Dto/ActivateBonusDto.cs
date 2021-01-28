using System;
using System.ComponentModel.DataAnnotations;

namespace ALG.Application.Services.Dto
{
    public class ActivateBonusDto
    {
        [Required]
        public Guid ServiceId { get; set; }
        [Required]
        public string Promocode { get; set; }
    }
}
