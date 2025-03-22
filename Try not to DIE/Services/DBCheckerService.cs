using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.ErrorResponse;

namespace Try_not_to_DIE.Services
{
    public class DBCheckerService
    {
        private readonly HospitalContext _context;

        public DBCheckerService(HospitalContext context)
        {
            _context = context;
        }

        public bool IsConnected()
        {
            if (_context.Database.CanConnect())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
