using System.ComponentModel.DataAnnotations;

namespace ALG.Application.Users.Dto
{
    public class CredentialsDto
    {
        /// <summary>
        /// Use Email as a login to the system
        /// </summary>
        [Required]
        public string Email { get; set;}
        [Required]
        public string Password { get; set; }
    }
}
