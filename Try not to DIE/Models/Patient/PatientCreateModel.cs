using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Patient
{
    public class PatientCreateModel
    {

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

    }
}
