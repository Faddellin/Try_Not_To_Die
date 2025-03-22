using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Diagnosis
{
    public class DiagnosRepository
    {
        private readonly HospitalContext _context;

        public DiagnosRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<DiagnosisDB>> getList()
        {
            return await _context.Diagnosis.ToListAsync();
        }

        public async Task<DiagnosisDB?> getById(Guid id)
        {
            return await _context.Diagnosis.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<DiagnosisDB> create(DiagnosisDB newDiagnos)
        {
            await _context.Diagnosis.AddAsync(newDiagnos);
            return newDiagnos;
        }
        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }
    }
}
