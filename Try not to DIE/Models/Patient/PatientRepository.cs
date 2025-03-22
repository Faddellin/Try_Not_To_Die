using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Patient
{
    public class PatientRepository
    {
        private readonly HospitalContext _context;

        public PatientRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<PatientDB>> getList()
        {
            return await _context.Patient.ToListAsync();
        }

        public async Task<PatientDB?> getById(Guid id)
        {
            return await _context.Patient.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<PatientDB> create(PatientDB newPatient)
        {
            await _context.Patient.AddAsync(newPatient);
            return newPatient;
        }

        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }

    }
}
