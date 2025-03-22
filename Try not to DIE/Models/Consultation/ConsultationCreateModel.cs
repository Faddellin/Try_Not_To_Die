using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Comment;

namespace Try_not_to_DIE.Models.Consultation
{
    public class ConsultationCreateModel()
    {
        [Required]
        public Guid specialityId { get; set; }

        [Required]
        public InspectionCommentCreateModel comment { get; set; }
    }
}
