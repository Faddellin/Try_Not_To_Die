using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Patient;

namespace Try_not_to_DIE.Models.Comment
{
    public class CommentRepository
    {
        private readonly HospitalContext _context;

        public CommentRepository(HospitalContext context)
        {
            _context = context;
        }

        public async Task<List<CommentDB>> getList()
        {
            return await _context.Comment.ToListAsync();
        }

        public async Task<CommentDB?> getById(Guid id)
        {
            return await _context.Comment.FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task<CommentDB> create(CommentDB newComment)
        {
            await _context.Comment.AddAsync(newComment);
            return newComment;
        }
        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
            return;
        }
    }
}
