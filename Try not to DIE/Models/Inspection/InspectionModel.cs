using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public DateTime date { get; set; }

        public string? anamnesis { get; set; }

        public string? complaints { get; set; }

        public string? treatment { get; set; }

        public Conclusion conclusion { get; set; }

        public DateTime? nextVisitDate { get; set; }

        public DateTime? deathDate { get; set; }

        public Guid? baseInspectionId { get; set; }

        public Guid? previousInspectionId { get; set; }

        public PatientModel patient { get; set; }

        public virtual DoctorModel doctor { get; set; }

        public List<DiagnosisModel>? diagnoses { get; set; }

        public List<InspectionConsultationModel>? consultations { get; set; }
    }
}
