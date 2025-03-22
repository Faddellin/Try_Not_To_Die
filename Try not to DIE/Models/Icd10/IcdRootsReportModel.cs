using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Icd10
{
    public class IcdRootsReportModel
    {
        public IcdRootsReportFiltersModel filters { get; set; }
        public List<IcdRootsReportRecordModel> records { get; set; }

        public Dictionary<string, int> summaryByRoot { get; set; }
    }
}
