using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Icd10
{
    public class IcdRootsReportFiltersModel
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public List<string> icdRoots { get; set; }
    }
}
