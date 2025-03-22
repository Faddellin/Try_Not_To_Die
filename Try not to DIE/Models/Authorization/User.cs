using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Models.Authorization
{
    public class User
    {

        public bool isAuthorizated()
        {
            return doctor == null ? false : true;
        }

        public DoctorDB? doctor { get; set; }

        public string token { get; set; }
    }
}
