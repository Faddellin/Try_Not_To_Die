using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class SpecialityService
    {
        private readonly SpecialityRepository _specialityRepository;

        public SpecialityService(SpecialityRepository specialityRepository)
        {
            _specialityRepository = specialityRepository;
        }

        public async Task<List<SpecialityModel>> GetAllSpecialitiesAsync()
        {
            return await _specialityRepository.getList();
        }

        public async Task<SpecialityModel> GetSpecialityByIdAsync(Guid id)
        {
            SpecialityModel? speciality = await _specialityRepository.getById(id);

            if (speciality == null)
            {
                throw new NotFoundException("Speciality not found");
            }

            return speciality;
        }

        public async Task<List<SpecialityModel>> GetSpecialtiesByName(string name)
        {
            List<SpecialityModel> specialties = await GetAllSpecialitiesAsync();

            specialties = specialties.Where(o => Regex.Match(o.name, Regex.Escape(name), RegexOptions.IgnoreCase).Success)
                                     .OrderBy(o => o.name).ToList();

            return specialties;
        }

    }
}
