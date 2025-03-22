using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Speciality;

namespace Try_not_to_DIE.Models.Doctor
{
    public class DoctorModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        [Phone]
        public string? phone { get; set; }

        [Required]
        public Guid specialityId { get; set; }

    }
}
