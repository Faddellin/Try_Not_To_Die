using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Speciality
{
    public class SpecialityModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }
    }
}
