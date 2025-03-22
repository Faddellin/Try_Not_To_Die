using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Services;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Comment;
using Microsoft.AspNetCore.Authorization;

namespace Try_not_to_DIE.Controllers
{

    [ApiController]
    [Route("api")]
    public class DictionaryController : ControllerBase
    {

        private readonly Icd10Service _icd10Service;
        private readonly SpecialityService _specialityService;
        private readonly DBCheckerService _dbCheckerService;
        private readonly PageService _pageService;

        public DictionaryController(Icd10Service icd10Service, SpecialityService specialityService, DBCheckerService dbCheckerService, PageService pageService)
        {
            _icd10Service = icd10Service;
            _specialityService = specialityService;
            _dbCheckerService = dbCheckerService;
            _pageService = pageService;
        }

        /// <summary>
        /// Get specialties list
        /// </summary>
        /// <param name="name">Part of the name for filtering</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Required number of elements per page</param>
        /// <response code="200">Specialties paged list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet]
        [ProducesResponseType(typeof(SpecialtiesPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("dictionary/speciality")]
        public async Task<IActionResult> GetSpecialityList(string name = "", int page = 1, int size = 5)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            List<SpecialityModel>? specialties = await _specialityService.GetSpecialtiesByName(name);

            SpecialtiesPagedListModel answer;
            try
            {
                answer = _pageService.CreateSpecialtiesPagedListModel(size, page, specialties);
            }
            catch (Models.Exceptions.BadRequest ex)
            {
                return BadRequest(new ResponseModel() { status = "Error", message = ex.Message });
            }
       

            return Ok(answer);
        }

        /// <summary>
        /// Import speciality list
        /// </summary>
        /// <response code="200">Specialties paged list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="500">InternalServerError</response>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(SpecialtiesPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("dictionary/speciality")]
        public async Task<IActionResult> ImportSpecialityList()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            await _specialityService.ImportSpecialities();

            return Ok();
        }


        /// <summary>
        /// Search for diagnoses in ICD-10 dictionary
        /// </summary>
        /// <param name="request">Part of the diagnosis name or code</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Required number of elements per page</param>
        /// <response code="200">Searching result extracted</response>
        /// <response code="400">Some fields in request are invalid</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet]
        [ProducesResponseType(typeof(Icd10SearchModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("dictionary/icd10")]
        public async Task<IActionResult> GetIcd10List(string request = "", int page = 1, int size = 5)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            List<Icd10DB>? Icd10 = await _icd10Service.GetIcd10ByNameAndCode(request);

            Icd10SearchModel answer;
            try
            {
                answer = _pageService.CreateIcd10SearchModel(size, page, Icd10);
            }
            catch (Models.Exceptions.BadRequest ex)
            {
                return BadRequest(new ResponseModel() { status = "Error", message = ex.Message });
            }
            

            return Ok(answer);

        }

        /// <summary>
        /// Get root ICD-10 elements
        /// </summary>
        /// <response code="200">Root ICD-10 elements retrieved</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Icd10RecordModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("dictionary/icd10/roots")]
        public async Task<IActionResult> GetIcd10RootsList()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            List<Icd10DB>? Icd10 = await _icd10Service.GetIcd10RootsAsync();

            List<Icd10RecordModel> neededIcd10 = new List<Icd10RecordModel>();

            foreach (var item in Icd10)
            {
                neededIcd10.Add(_icd10Service.MapToIcd10RecordModel(item));
            }

            return Ok(neededIcd10);
        }

        /// <summary>
        /// Import Icd-10 dictionary
        /// </summary>
        /// <response code="200">Root ICD-10 elements retrieved</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(List<Icd10RecordModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("dictionary/icd10")]
        public async Task<IActionResult> ImportIcd10()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            await _icd10Service.ImportIcd10();


            return Ok();
        }

    }
}
