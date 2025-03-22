using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Patient
{
    public class PatientModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime? createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

    }
}
