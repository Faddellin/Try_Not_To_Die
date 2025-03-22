using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Diagnosis
{
    public class DiagnosisCreateModel
    {

        [Required]
        public Guid icdDiagnosisId { get; set; }

        [MaxLength(5000)]
        public string? description { get; set; }

        [Required]
        public DiagnosisType type { get; set; }
    }
}
