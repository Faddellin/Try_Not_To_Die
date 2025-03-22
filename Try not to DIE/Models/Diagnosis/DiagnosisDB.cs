using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Diagnosis
{
    public class DiagnosisDB
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }


        virtual public Icd10DB icd10 {  get; set; }

        public string? description { get; set; }

        [Required]
        public DiagnosisType type { get; set; }
    }
}
