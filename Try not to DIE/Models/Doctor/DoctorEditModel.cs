using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Doctor
{
    public class DoctorEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

        [Phone]
        public string? phone { get; set; }
    }
}
