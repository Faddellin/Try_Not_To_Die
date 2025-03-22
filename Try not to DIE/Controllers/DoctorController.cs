using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Controllers
{
    [ApiController]
    [Route("api")]
    public class DoctorController : ControllerBase
    {

        private readonly JwtService _jwtService;
        private readonly DBCheckerService _dbCheckerService;
        private readonly UserService _userService;
        private readonly SpecialityService _specialityService;
        private readonly DoctorService _doctorService;
        private readonly TokenService _tokenService;


        public DoctorController(UserService userService, JwtService jwtService, DBCheckerService dbCheckerService, SpecialityService specialityService,
                                DoctorService doctorService, TokenService tokenService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _dbCheckerService = dbCheckerService;
            _specialityService = specialityService;
            _doctorService = doctorService;
            _tokenService = tokenService;
        }

        
        /// <summary>
        /// Register new user
        /// </summary>
        /// <response code="200">Doctcor was registered</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Route("doctor/register")]
        public async Task<IActionResult> CreateDoctor(DoctorRegisterModel doctor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            SpecialityModel speciality;
            try
            {
                speciality = await _specialityService.GetSpecialityByIdAsync(doctor.speciality);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            DoctorDB newDoctor;
            try
            {
                await _doctorService.GetDoctorByEmailAsync(doctor.email);
                return StatusCode(403, new ResponseModel() { status = "Error", message = "Doctor with this email already exists" });
            }
            catch (NotFoundException ex)
            {
                newDoctor = await _doctorService.AddDoctorAsync(doctor, speciality);
            }

            return Ok(new TokenResponseModel(_jwtService.GenerateToken(newDoctor)));
        }


        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <response code="200">Doctor was logined</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("doctor/login")]
        [ProducesResponseType(typeof(TokenResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Login(LoginCredentialsModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!_dbCheckerService.IsConnected())
            {
                return StatusCode(500, new ResponseModel() { status = "Error", message = "Couldn't connect to the database" });
            }

            DoctorDB curDoctor;
            try
            {
                curDoctor = await _doctorService.GetDoctorByEmailAsync(model.email);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }


            if (curDoctor.password == model.password)
            {
                return Ok(new TokenResponseModel(_jwtService.GenerateToken(curDoctor)));
            }
            else
            {
                return Unauthorized(new ResponseModel() { status = "Error", message = "Not correct password" });
            }

            
        }


        /// <summary>
        /// Log out system user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet("doctor/logout")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Logout()
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

            _tokenService.BlacklistToken(user.token);

            return Ok(new ResponseModel() { status = "Error", message = "User was logout" });

        }


        /// <summary>
        /// Get user profile
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpGet("doctor/profile")]
        [ProducesResponseType(typeof(DoctorModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetProfile()
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

            return Ok(_doctorService.MapToDoctorModel(user.doctor));

        }


        /// <summary>
        /// Edit user profile
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [Authorize]
        [HttpPut("doctor/profile")]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Edit(DoctorEditModel editedDoctor)
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
                DoctorDB doctorCheck = await _doctorService.GetDoctorByEmailAsync(editedDoctor.email);

                if (doctorCheck != user.doctor)
                {
                    return BadRequest(new ResponseModel() { status = "Error", message = "User with this email already exists" });
                }
            }
            catch (NotFoundException ex)
            {
            }
            
            try
            {
                await _doctorService.EditDoctorAsync(user.doctor.id, editedDoctor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseModel() { status = "Error", message = ex.Message });
            }
            

            return Ok();

        }


    }
}
