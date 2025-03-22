using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Inspection;

namespace Try_not_to_DIE.Models.Patient
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patients { get; set; }

        public PageInfoModel pagination { get; set; }

    }
}
