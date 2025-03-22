using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Numerics;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class UserService
    {
        private readonly DoctorService _doctorService;
        private readonly TokenService _tokenService;
        private readonly JwtService _jwtService;


        public UserService(DoctorService doctorService, TokenService tokenService, JwtService jwtService)
        {
            _doctorService = doctorService;
            _tokenService = tokenService;
            _jwtService = jwtService;
        }

        public async Task<User> FindUserByRequest(HttpRequest request)
        {
            User user = new User();

            user.token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenService.IsTokenValid(user.token))
            {
                Guid doctorId = _jwtService.GetIdFromToken(user.token);
                user.doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            }
            return user;
        }
        
    }
}
