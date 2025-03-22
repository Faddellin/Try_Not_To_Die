using System.ComponentModel.DataAnnotations;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Models.ErrorResponse
{
    public class ResponseModel
    {
        public string? status { get; set; }

        public string? message { get; set; }
    }
}
