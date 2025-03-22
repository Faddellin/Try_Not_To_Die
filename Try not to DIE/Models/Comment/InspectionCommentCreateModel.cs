using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Comment
{
    public class InspectionCommentCreateModel
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string content { get; set; }
    }
}
