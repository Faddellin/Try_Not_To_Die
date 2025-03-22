using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Icd10.Icd10ImportModels;

public class Icd10HelpModel
{
    public Guid id { get; set; }
    public int intId { get; set; }

    public DateTime createTime { get; set; }

    public string code { get; set; }

    public string name { get; set; }

    public int? parentIntId { get; set; }

    public Guid? parentId { get; set; }

    public Guid? rootId { get; set; }

}
