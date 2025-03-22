using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Inspection
{
    public class InspectionRepository
    {
        private readonly HospitalContext _context;

        public InspectionRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<InspectionDB>> getList()
        {
            return await _context.Inspection.ToListAsync();
        }

        public async Task<InspectionDB?> getById(Guid id)
        {
            return await _context.Inspection.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<InspectionDB> create(InspectionDB newInspection)
        {
            await _context.Inspection.AddAsync(newInspection);
            return newInspection;
        }

        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }

    }
}
