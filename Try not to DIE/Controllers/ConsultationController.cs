using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Migrations;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Controllers
{

    [ApiController]
    [Route("api")]
    public class ConsultationController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly DBCheckerService _dbCheckerService;
        private readonly PatientService _patientService;
        private readonly PageService _pageService;
        private readonly InspectionService _inspectionService;
        private readonly ConsultationService _consultationService;
        private readonly CommentService _commentService;

        public ConsultationController(UserService userService, DBCheckerService dbCheckerService, PatientService patientService, PageService pageService,
                                 InspectionService inspectionService, ConsultationService consultationService, CommentService commentService)
        {
            _userService = userService;
            _dbCheckerService = dbCheckerService;
            _patientService = patientService;
            _pageService = pageService;
            _inspectionService = inspectionService;
            _consultationService = consultationService;
            _commentService = commentService;
        }

        /// <summary>
        /// Get a list of medical inspections for consultation
        /// </summary>
        /// <param name="grouped">Flag - whether grouping by inspection chain is required - for filtration</param>
        /// <param name="icdRoots">Root elements for ICD-10 - for filtration</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Required number of elements per page</param>
        /// <response code="200">Inspections for consultation list retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [Route("consultation")]
        [ProducesResponseType(typeof(InspectionPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetInspections(bool grouped, [FromQuery] List<Guid> icdRoots, int page = 1, int size = 5)
        {

            User user = await _userService.FindUserByRequest(Request);

            if (!user.isAuthorizated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            List<InspectionDB> filteredInspections;
            try
            {
                filteredInspections = await _inspectionService.GetInspectionsByFiltersAsync(grouped, icdRoots);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }

            InspectionPagedListModel inspectionPage = _pageService.CreateInspectionPagedListModel(size, page, filteredInspections);
            
            return Ok(inspectionPage);
        }


        /// <summary>
        /// Get concrete consultation
        /// </summary>
        /// <param name="id">Consultation's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [Route("consultation/{id}")]
        [ProducesResponseType(typeof(ConsultationModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetConsultation(Guid id)
        {
            User user = await _userService.FindUserByRequest(Request);

            if (!user.isAuthorizated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            ConsultationDB consultation;
            try
            {
                consultation = await _consultationService.GetConsultationByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
             

            return Ok(_consultationService.MapToConsultationModel(consultation));
        }


        /// <summary>
        /// Add comment to concrete consultation
        /// </summary>
        /// <param name="id">Consultation's identifier</param>
        /// <param name="comment">Comment model</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User doesn't have add comment to consultation (unsuitable specialty and not the inspection author)</response>
        /// <response code="404">Consultation or parent comment not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPost]
        [Route("consultation/{id}/comment")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreateComment(Guid id, CommentCreateModel comment)
        {

            User user = await _userService.FindUserByRequest(Request);

            if (!user.isAuthorizated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            CommentDB newComment;
            try
            {
                newComment = await _consultationService.AddCommentToConsultationAsync(id, comment, user.doctor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ResponseModel() { status = "Error", message = ex.Message });
            }

            return Ok(newComment.id);
        }


        /// <summary>
        /// Edit comment
        /// </summary>
        /// <param name="id">Comment's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User is not the author of the comment</response>
        /// <response code="404">Comment not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPut]
        [Route("consultation/comment/{id}")]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> EditComment(Guid id, InspectionCommentCreateModel newCommentContent)
        {

            User user = await _userService.FindUserByRequest(Request);

            if (!user.isAuthorizated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            CommentDB comment;
            try
            {
                comment = await _commentService.EditCommentAsync(id, newCommentContent.content, user.doctor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ResponseModel() { status = "Error", message = ex.Message });
            }


            return Ok();
        }
    }
}
