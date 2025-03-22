using System.ComponentModel.DataAnnotations;

namespace Try_not_to_DIE.Models.Authorization
{
    public class TokenResponseModel
    {

        public TokenResponseModel() { }

        public TokenResponseModel(string token)
        {
            this.token = token;
        }

        [Required]
        [MinLength(1)]
        public string token { get; set; }
    }
}
