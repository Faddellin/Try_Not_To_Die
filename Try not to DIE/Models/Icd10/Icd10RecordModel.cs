using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Icd10
{
    public class Icd10RecordModel
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public string? code { get; set; }

        public string? name { get; set; }
    }
}
