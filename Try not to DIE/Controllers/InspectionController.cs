using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Controllers
{

    [ApiController]
    [Route("api")]
    public class InspectionController : ControllerBase
    {

        private readonly InspectionService _inspectionService;
        private readonly UserService _userService;
        private readonly DBCheckerService _dbCheckerService;

        public InspectionController(InspectionService inspectionService, UserService userService, DBCheckerService dbCheckerService)
        {
            _inspectionService = inspectionService;
            _userService = userService;
            _dbCheckerService = dbCheckerService;
        }


        /// <summary>
        /// Get full information about specified inspection fff
        /// </summary>
        /// <param name="id">Inspection's identifier</param>
        /// <response code="200">Inspection found and successfully extracted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(InspectionModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("inspection/{id}")]
        public async Task<IActionResult> GetInspection(Guid id)
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

            InspectionDB inspection;
            
            try
            {
                 inspection = await _inspectionService.GetInspectionByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message});
            }

            InspectionModel answer = _inspectionService.MapToInspectionModel(inspection);

            return Ok(answer);
        }


        /// <summary>
        /// Edit concrete inspection
        /// </summary>
        /// <param name="id">Inspection's identifier</param>
        /// <param name="changedInspection">Inspection model</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User doesn't have editing rights (not the inspection author)</response>
        /// <response code="404">Inspection not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("inspection/{id}")]
        public async Task<IActionResult> EditInspection(Guid id, InspectionEditModel changedInspection)
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

            try
            {
                await _inspectionService.EditInspectionAsync(id, changedInspection, user.doctor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            catch (Models.Exceptions.BadRequest ex)
            {
                return BadRequest(new ResponseModel() { status = "Error", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ResponseModel() { status = "Error", message = ex.Message });
            }

            return Ok();
        }


        /// <summary>
        /// Get medical inspection chain for root inspection
        /// </summary>
        /// <param name="id">Root inspection's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<InspectionPreviewModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("inspection/{id}/chain")]
        public async Task<IActionResult> GetInspectionChain(Guid id)
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

            List<InspectionPreviewModel> inspectionChain = new List<InspectionPreviewModel>();
            List<InspectionDB>? inspectionDBChain;
            try
            {
                inspectionDBChain = await _inspectionService.GetInspectionChain(id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
             
            foreach (var inspection in inspectionDBChain)
            {
                inspectionChain.Add(_inspectionService.MapToInspectionPreviewModel(inspection));
            }

            return Ok(inspectionChain);
        }
    }
}
