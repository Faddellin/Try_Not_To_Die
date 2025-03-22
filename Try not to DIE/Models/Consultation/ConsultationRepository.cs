using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Consultation
{
    public class ConsultationRepository
    {
        private readonly HospitalContext _context;

        public ConsultationRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<ConsultationDB>> getList()
        {
            return await _context.Consultation.ToListAsync();
        }

        public async Task<ConsultationDB?> getById(Guid id)
        {
            return await _context.Consultation.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<ConsultationDB> create(ConsultationDB newConsultation)
        {
            await _context.Consultation.AddAsync(newConsultation);
            return newConsultation;
        }

        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }
    }
}
