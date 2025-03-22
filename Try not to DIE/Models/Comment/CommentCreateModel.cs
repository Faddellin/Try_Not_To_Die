using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Comment
{
    public class CommentCreateModel
    {
        [Required]
        [StringLength(1000)]
        public string content { get; set; }

        public Guid parentId { get; set; }
    }
}
