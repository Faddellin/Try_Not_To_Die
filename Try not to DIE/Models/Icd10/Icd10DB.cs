using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models
{
    public class Icd10DB
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        virtual public Icd10DB? parent { get; set; }

        public Guid? parentId { get; set; }

        public Guid rootId { get; set; }
        //Если это icd является корнем, то rootId = id

    }
}
