using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Controllers
{

    [ApiController]
    [Route("api")]
    public class ReportController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DBCheckerService _dbCheckerService;
        private readonly ReportService _reportService;

        public ReportController(UserService userService, DBCheckerService dbCheckerService, ReportService reportService)
        {
            _userService = userService;
            _dbCheckerService = dbCheckerService;
            _reportService = reportService;
        }


        /// <summary>
        /// Get a report on patients' visits based on ICD-10 roots for a specified time interval
        /// </summary>
        /// <param name="start">Start of tome interval</param>
        /// <param name="end">End of time interval</param>
        /// <param name="icdRoots">Set of ICD-10 roots. All possible roots if null</param>
        /// <response code="200">Report extracted successfully</response>
        /// <response code="400">Some fields in request are invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IcdRootsReportModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("report/icdrootsreport")]
        public async Task<IActionResult> CreateIcdRootsReport(DateTime start, DateTime end, [FromQuery] List<Guid> icdRoots)
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

            IcdRootsReportModel answer;
            try
            {
                answer = await _reportService.CreateIcdRootsReportModel(start, end, icdRoots);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }

            return Ok(answer);

        }


    }
}
