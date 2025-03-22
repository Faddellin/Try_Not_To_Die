using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Speciality;

namespace Try_not_to_DIE.Models.Consultation
{
    public class ConsultationDB
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        virtual public InspectionDB inspectionDB { get; set; }

        virtual public SpecialityModel speciality { get; set; }

        virtual public List<CommentDB> comments { get; set; }
    }
}
