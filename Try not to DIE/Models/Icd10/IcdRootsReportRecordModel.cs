using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Icd10
{
    public class IcdRootsReportRecordModel
    {
        public string? patientName { get; set; }
        public DateTime? patientBirthdate { get; set; }
        public Gender gender { get; set; }

        public Dictionary<string, int> visitsByRoot {  get; set; } 
    }
}
