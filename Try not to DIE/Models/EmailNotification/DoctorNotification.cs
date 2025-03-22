using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.EmailNotification
{
    public class DoctorNotification
    {
        public Guid inspectionID { get; set; }

        public DateTime nextVisitDate { get; set; }
    }
}
