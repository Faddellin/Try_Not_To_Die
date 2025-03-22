using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Doctor;

namespace Try_not_to_DIE.Models.Comment
{
    public class CommentDB
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public DateTime? modifiedDate { get; set; }

        [Required]
        [MinLength(1)]
        public string content { get; set; }

        [Required]
        virtual public DoctorDB author { get; set; }


        virtual public CommentDB? parent { get; set; }
    }
}
