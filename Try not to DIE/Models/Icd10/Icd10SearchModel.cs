using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Icd10
{
    public class Icd10SearchModel
    {
        public List<Icd10RecordModel> records { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
