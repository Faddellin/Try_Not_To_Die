using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Speciality;

namespace Try_not_to_DIE.Models.Consultation
{
    public class InspectionConsultationModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid inspectionId { get; set; }

        public SpecialityModel speciality { get; set; }

        public InspectionCommentModel rootComment { get; set; }

        public int commentsNumber { get; set; }

    }
}
