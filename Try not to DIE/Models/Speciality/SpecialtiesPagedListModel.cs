using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Speciality
{
    public class SpecialtiesPagedListModel
    {

        public List<SpecialityModel> specialties { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
