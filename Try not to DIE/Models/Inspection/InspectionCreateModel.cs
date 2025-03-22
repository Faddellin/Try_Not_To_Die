using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionCreateModel
    {
        /// <summary>
        /// Date and time of the inspection (UTC)
        /// </summary>
        [Required]
        public DateTime date { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string anamnesis { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string complaints { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string treatment { get; set; }

        public Conclusion conclusion { get; set; }

        /// <summary>
        /// Date and time of the next visit in case of Disease conclusion (UTC)
        /// </summary>
        public DateTime? nextVisitDate { get; set; }

        /// <summary>
        /// Date and time of the death in case of Death conclusion (UTC)
        /// </summary>
        public DateTime? deathDate { get; set; }

        public Guid? previousInspectionId { get; set; }

        [Required]
        [MinLength(1)]
        public List<DiagnosisCreateModel> diagnoses { get; set; }

        public List<ConsultationCreateModel>? consultations { get; set; }
    }
}
