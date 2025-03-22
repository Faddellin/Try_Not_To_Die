using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionDB
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

        public Guid? nextInspectionId { get; set; }

        virtual public PatientDB patient { get; set; }

        virtual public DoctorDB doctor { get; set; }

        virtual public List<DiagnosisDB> diagnoses { get; set; }

        virtual public List<ConsultationDB> consultations { get; set; }

        public bool hasChain {  get; set; }

        public bool hasNested { get; set; }

    }
}
