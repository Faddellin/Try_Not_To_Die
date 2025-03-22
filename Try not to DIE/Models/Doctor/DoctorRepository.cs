using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Doctor
{
    public class DoctorRepository
    {
        private readonly HospitalContext _context;

        public DoctorRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorDB>> getList()
        {
            return await _context.Doctor.ToListAsync();
        }

        public async Task<DoctorDB?> getById(Guid id)
        {
            return await _context.Doctor.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<DoctorDB?> getByEmail(string email)
        {
            return await _context.Doctor.FirstOrDefaultAsync(o => o.email == email);
        }

        public async Task<DoctorDB> create(DoctorDB newDoctor)
        {
            await _context.Doctor.AddAsync(newDoctor);
            return newDoctor;
        }
        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }
    }
}
