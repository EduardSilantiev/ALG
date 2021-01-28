using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALG.Core.Services
{
    [Table("Service")]
    public partial class Service
    {
        public Service()
        {
            ActivatedBonuses = new HashSet<ActivatedBonus>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Promocode { get; set; }
        [StringLength(200)]
        public string Description { get; set; }

        [InverseProperty(nameof(ActivatedBonus.Service))]
        public virtual ICollection<ActivatedBonus> ActivatedBonuses { get; set; }
    }
}
