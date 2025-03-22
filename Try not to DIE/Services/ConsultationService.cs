using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class ConsultationService
    {
        private readonly ConsultationRepository _consultationRepository;
        private readonly CommentService _commentService;

        public ConsultationService(ConsultationRepository consultationRepository, CommentService commentService)
        {
            _consultationRepository = consultationRepository;
            _commentService = commentService;
        }

        public async Task<List<ConsultationDB>> GetAllConsultationsAsync()
        {
            return await _consultationRepository.getList();
        }

        public async Task<ConsultationDB> GetConsultationByIdAsync(Guid id)
        {
            ConsultationDB? consultation = await _consultationRepository.getById(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found");
            }
            return consultation;
        }

        public async Task<ConsultationDB> CreateConsultationAsync(ConsultationCreateModel consultation, InspectionDB inspection, SpecialityModel speciality, DoctorDB doctor)
        {
            List<CommentDB> newComments = new List<CommentDB>();

            CommentDB newComment = await _commentService.CreateCommentAsync(consultation.comment.content, doctor, null);

            newComments.Add(newComment);

            ConsultationDB newConsultation = new ConsultationDB()
            {
                id = new Guid(),
                createTime = DateTime.Now,
                inspectionDB = inspection,
                speciality = speciality,
                comments = newComments
            };
            await _consultationRepository.create(newConsultation);

            return newConsultation;
        }

        public async Task<CommentDB> AddCommentToConsultationAsync(Guid id, CommentCreateModel comment, DoctorDB doctor)
        {
            ConsultationDB consultation = await GetConsultationByIdAsync(id);

            if ((consultation.speciality.id != doctor.speciality.id) && (doctor.id != consultation.comments.First().author.id))
            {
                throw new ForbiddenException("You have an unsuitable specialty for commenting");
            }

            CommentDB? parentComment = consultation.comments.FirstOrDefault(o => o.id == comment.parentId);
            if (parentComment == null)
            {
                throw new NotFoundException("Parent comment not found");
            }
            
            CommentDB newComment = await _commentService.CreateCommentAsync(comment.content, doctor, parentComment);

            consultation.comments.Add(newComment);

            await _consultationRepository.saveChanges();

            return newComment;
        }

        public InspectionConsultationModel MapToInspectionConsultationModel(ConsultationDB consultation)
        {
            InspectionConsultationModel answer = new InspectionConsultationModel() {
                id = consultation.id,
                createTime = consultation.createTime,
                inspectionId = consultation.inspectionDB.id,
                speciality = consultation.speciality,
                rootComment = _commentService.MapToInspectionCommentModel(consultation.comments[0]),
                commentsNumber = consultation.comments.Count()
            };

            return answer;
        }

        public ConsultationModel MapToConsultationModel(ConsultationDB consultation)
        {
            ConsultationModel answer = new ConsultationModel()
            {
                id = consultation.id,
                createTime = consultation.createTime,
                inspectionId = consultation.inspectionDB.id,
                speciality = consultation.speciality,
                comments = new List<CommentModel>()
            };
            
            foreach (var comment in consultation.comments)
            {
                answer.comments.Add(_commentService.MapToCommentModel(comment));
            }

            return answer;
        }
    }
}
