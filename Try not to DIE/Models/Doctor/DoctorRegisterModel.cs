using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Doctor
{
    public class DoctorRegisterModel
    {

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string name { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

        [Phone]
        public string? phone { get; set; }

        [Required]
        public Guid speciality { get; set; }
    }
}
