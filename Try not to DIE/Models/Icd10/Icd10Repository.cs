using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;

namespace Try_not_to_DIE.Models.Icd10
{
    public class Icd10Repository
    {
        private readonly HospitalContext _context;

        public Icd10Repository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<Icd10DB>> getList()
        {
            return await _context.Icd10.ToListAsync();
        }

        public async Task<Icd10DB?> getById(Guid id)
        {
            return await _context.Icd10.FirstOrDefaultAsync(o => o.id == id);
        }

    }
}
