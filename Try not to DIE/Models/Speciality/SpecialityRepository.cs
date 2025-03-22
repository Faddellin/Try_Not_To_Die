using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;

namespace Try_not_to_DIE.Models.Speciality
{
    public class SpecialityRepository
    {
        private readonly HospitalContext _context;

        public SpecialityRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<SpecialityModel>> getList()
        {
            return await _context.Speciality.ToListAsync();
        }

        public async Task<SpecialityModel?> getById(Guid id)
        {
            return await _context.Speciality.FirstOrDefaultAsync(o => o.id == id);
        }

    }
}
