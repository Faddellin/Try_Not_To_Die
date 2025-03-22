using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Inspection;

namespace Try_not_to_DIE.Models.Patient
{
    public class PatientDB
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

        virtual public List<InspectionDB> allInspections { get; set; }

    }
}
