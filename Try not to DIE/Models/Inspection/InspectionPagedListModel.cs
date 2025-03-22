using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionPagedListModel
    {
        public List<InspectionPreviewModel>? inspections { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
