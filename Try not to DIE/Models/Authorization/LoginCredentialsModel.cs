using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Authorization
{
    public class LoginCredentialsModel
    {

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        public string password { get; set; }

    }
}