using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Doctor;

namespace Try_not_to_DIE.Models.Comment
{
    public class CommentModel
    {

        public CommentModel() { }
        public CommentModel(CommentDB comment)
        {
            this.id = comment.id;
            this.createTime = comment.createTime;
            this.content = comment.content;
            this.authorId = comment.author.id;
            this.author = comment.author.name;

            if (comment.parent != null)
            {
                this.parentId = comment.parent.id;
            }
            
        }

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public DateTime? modifiedDate { get; set; }

        [Required]
        [MinLength(1)]
        public string content { get; set; }

        [Required]
        public Guid authorId { get; set; }

        [Required]
        [MinLength(1)]
        public string author { get; set; }

        public Guid? parentId { get; set; }
    }
}
