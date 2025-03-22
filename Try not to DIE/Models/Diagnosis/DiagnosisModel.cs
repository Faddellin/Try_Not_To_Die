using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Diagnosis
{
    public class DiagnosisModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string code { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        public string? description { get; set; }

        [Required]
        public DiagnosisType type { get; set; }
    }
}
