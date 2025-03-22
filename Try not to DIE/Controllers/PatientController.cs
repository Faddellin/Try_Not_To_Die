using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Controllers
{

    [ApiController]
    [Route("api")]
    public class PatientController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly DBCheckerService _dbCheckerService;
        private readonly PatientService _patientService;
        private readonly PageService _pageService;
        private readonly InspectionService _inspectionService;

        public PatientController(UserService userService, DBCheckerService dbCheckerService, PatientService patientService, PageService pageService, 
                                 InspectionService inspectionService)
        {
            _userService = userService;
            _dbCheckerService = dbCheckerService;
            _patientService = patientService;
            _pageService = pageService;
            _inspectionService = inspectionService;
        }

        /// <summary>
        /// Create new patient
        /// </summary>
        /// <response code="200">Patient was registered</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient")]
        public async Task<IActionResult> CreatePatient(PatientCreateModel patient)
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

            PatientDB newPatient = await _patientService.AddPatientAsync(patient);

            return Ok(newPatient.id);
        }


        /// <summary>
        /// Get patients list
        /// </summary>
        /// <param name="name">Part of the name for filtering</param>
        /// <param name="conclusions">Conclusion list to filter by conclusions</param>
        /// <param name="sorting">Option to sort patients</param>
        /// <param name="scheduledVisits">Show only scheduled visits</param>
        /// <param name="onlyMine">Show inspections done by this doctor</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Required number of elements per page</param>
        /// <response code="200">Patients paged list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination/sorting</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PatientPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient")]
        public async Task<IActionResult> GetPatients(string? name, [FromQuery] List<Conclusion> conclusions, PatientSorting? sorting, bool scheduledVisits
                                                    , bool onlyMine,  int page = 1, int size = 5)
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
            if (name == null)
            {
                name = "";
            }

            List<PatientDB> patientsSortedByFilterd = await _patientService.GetPatientsByFiltersAsync(name, conclusions, sorting, scheduledVisits, onlyMine, user.doctor);

            PatientPagedListModel answer = _pageService.CreatePatientPagedListModel(size, page, patientsSortedByFilterd);

            return Ok(answer);
        }


        /// <summary>
        /// Create inspection for specified patient
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <param name="inspection">Inspection model</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient/{id}/inspections")]
        public async Task<IActionResult> CreateInspection(Guid id, InspectionCreateModel inspection)
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

            InspectionDB newInspection;
            try
            {
                newInspection = await _inspectionService.AddInspectionAsync(inspection, id, user.doctor.id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            catch (Models.Exceptions.BadRequest ex)
            {
                return BadRequest(new ResponseModel() { status = "Error", message = ex.Message });
            }

            return Ok(newInspection.id);
        }


        /// <summary>
        /// Get a list of patient medical inspections
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <param name="grouped">Flag - whether grouping by inspection chain is required - for filtration</param>
        /// <param name="icdRoots">Root elements for ICD-10 - for filtration</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Required number of elements per page</param>
        /// <response code="200">Patients inspections list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Patient not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(InspectionPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient/{id}/inspections")]
        public async Task<IActionResult> GetPatientInspections(Guid id, bool grouped, [FromQuery] List<Guid> icdRoots, int page = 1, int size = 5)
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
                filteredInspections = await _patientService.GetPatientInspectionsByFiltersAsync(id, icdRoots, grouped);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }

            InspectionPagedListModel answer = _pageService.CreateInspectionPagedListModel(size, page, filteredInspections);

            return Ok(answer);
        }


        /// <summary>
        /// Search for patient medical inspections without child inspections
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <param name="request">Part of the diagnosis name or code</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<InspectionShortModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient/{id}/inspections/search")]
        public async Task<IActionResult> GetPatientInspectionsWithoutChild(Guid id, string request = "")
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

            List<InspectionDB> inspections;
            try
            {
                inspections = await _patientService.GetPatientInspectionsWithoutChildByRequestAsync(id, request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            
            List<InspectionShortModel> neededInspections = new List<InspectionShortModel>();
            foreach (var inspection in inspections)
            {
                neededInspections.Add(_inspectionService.MapToInspectionShortModel(inspection));
            }

            return Ok(neededInspections);
        }


        /// <summary>
        /// Get patient card
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PatientModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("patient/{id}")]
        public async Task<IActionResult> GetPatient(Guid id)
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

            PatientDB patient;
            try
            {
                patient = await _patientService.GetPatientByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }


            return Ok(_patientService.MapToPatientModel(patient));
        }

    }
}
