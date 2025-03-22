using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionShortModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public DiagnosisModel diagnosis { get; set; }

    }
}