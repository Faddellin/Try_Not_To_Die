using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Doctor;

namespace Try_not_to_DIE.Models.Comment
{
    public class InspectionCommentModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid? parentId { get; set; }

        public string? content { get; set; }

        public DoctorModel author { get; set; }

        public DateTime? modifyTime { get; set; }

    }
}
