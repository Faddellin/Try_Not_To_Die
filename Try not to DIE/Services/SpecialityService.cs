using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Models.Speciality.SpecialityImportModels;
using Try_not_to_DIE.ServicesInterfaces;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class SpecialityService
    {

        private readonly IOptions<AppConfig> _config;
        private readonly IJsonReaderService _jsonReaderService;
        private readonly HospitalContext _context;

        public SpecialityService(IOptions<AppConfig> config, IJsonReaderService jsonReaderService, HospitalContext context)
        {
            _config = config;
            _jsonReaderService = jsonReaderService;
            _context = context;
        }


        public async Task<SpecialityModel> GetSpecialityByIdAsync(Guid id)
        {
            SpecialityModel? speciality = await _context.Speciality.FirstOrDefaultAsync(o => o.id == id);

            if (speciality == null)
            {
                throw new NotFoundException("Speciality not found");
            }

            return speciality;
        }

        public async Task ImportSpecialities()
        {
            string pathToFile = Directory.GetCurrentDirectory() + _config.Value.SpecialitiesFilePath;
            SpecialityImportListModel importModel = await _jsonReaderService.GetJsonDataAsync<SpecialityImportListModel>(pathToFile);

            await _context.Speciality.ExecuteDeleteAsync();

            foreach (var item in importModel.specialities)
            {
                await _context.Speciality.AddAsync(new SpecialityModel()
                {
                    createTime = DateTime.Now,
                    id = Guid.NewGuid(),
                    name = item.name
                });
            }

            await _context.SaveChangesAsync();

            return;
        }

        public async Task<List<SpecialityModel>> GetSpecialtiesByName(string name)
        {
            List<SpecialityModel> specialties = await _context.Speciality.Where(o => Regex.Match(o.name, Regex.Escape(name), RegexOptions.IgnoreCase).Success)
                                     .OrderBy(o => o.name).ToListAsync();

            return specialties;
        }

    }
}
