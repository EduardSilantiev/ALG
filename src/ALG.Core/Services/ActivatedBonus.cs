using ALG.Core.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALG.Core.Services
{
    public partial class ActivatedBonus
    {
        [Key]
        public Guid UserId { get; set; }
        [Key]
        public Guid ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        [InverseProperty("ActivatedBonuses")]
        public virtual Service Service { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("ActivatedBonuses")]
        public virtual User User { get; set; }
    }
}
