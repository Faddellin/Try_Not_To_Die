using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Try_not_to_DIE.Configuration;

public class AppConfig
{
    public AuthConfig authConfig {  get; set; }
    public string Icd10FilePath { get; set; }

}
