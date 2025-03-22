using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepository;
        private readonly DoctorService _doctorService;

        public CommentService(CommentRepository commentRepository, DoctorService doctorService)
        {
            _commentRepository = commentRepository;
            _doctorService = doctorService;
        }

        public async Task<List<CommentDB>> GetAllCommentsAsync()
        {
            return await _commentRepository.getList();
        }

        public async Task<CommentDB?> GetCommentByIdAsync(Guid id)
        {
            return await _commentRepository.getById(id);
        }
        public async Task<CommentDB> CreateCommentAsync(string commentContent, DoctorDB doctor, CommentDB? parent)
        {
            CommentDB newComment = new CommentDB()
            {
                id = new Guid(),
                createTime = DateTime.Now,
                content = commentContent,
                author = doctor,
                parent = parent
            };
            await _commentRepository.create(newComment);

            return newComment;
        }

        public async Task<CommentDB> EditCommentAsync(Guid id, string newCommentContent, DoctorDB doctor)
        {
            CommentDB? comment = await GetCommentByIdAsync(id);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found");
            }
            if (comment.author != doctor)
            {
                throw new ForbiddenException("You're not comment author");
            }

            comment.content = newCommentContent;

            await _commentRepository.saveChanges();

            return comment;
        }
        public InspectionCommentModel MapToInspectionCommentModel(CommentDB comment)
        {
            InspectionCommentModel answer = new InspectionCommentModel() {
                id = comment.id,
                createTime = comment.createTime,
                content = comment.content,
                author = _doctorService.MapToDoctorModel(comment.author),
                modifyTime = comment.modifiedDate,
            };

            if (comment.parent != null)
            {
                answer.parentId = comment.parent.id;
            }

            return answer;
        }

        public CommentModel MapToCommentModel(CommentDB comment)
        {
            CommentModel answer = new CommentModel()
            {
                id = comment.id,
                createTime = comment.createTime,
                content = comment.content,
                authorId = comment.author.id,
                author = comment.author.name
            };
            
            if (comment.parent != null)
            {
                answer.parentId = comment.parent.id;
            }

            return answer;
        }
    }
}
