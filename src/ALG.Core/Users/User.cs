using ALG.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALG.Core.Users
{
    [Table("User")]
    public partial class User
    {
        public User()
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
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [InverseProperty(nameof(ActivatedBonus.User))]
        public virtual ICollection<ActivatedBonus> ActivatedBonuses { get; set; }
    }

}
