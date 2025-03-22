using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.RegularExpressions;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Icd10.Icd10ImportModels;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.ServicesInterfaces;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class Icd10Service
    {
        private readonly IOptions<AppConfig> _config;
        private readonly IJsonReaderService _jsonReaderService;
        private readonly HospitalContext _context;

        public Icd10Service(Icd10Repository icd10Repository, IOptions<AppConfig> config, IJsonReaderService jsonReaderService, HospitalContext context)
        {
            _config = config;
            _jsonReaderService = jsonReaderService;
            _context = context;
        }

        public async Task<List<Icd10DB>> GetAllIcd10Async()
        {
            return await _context.Icd10.ToListAsync();
        }

        public async Task<Icd10DB> GetIcd10ByIdAsync(Guid id)
        {
            Icd10DB? answer = await _context.Icd10.FirstOrDefaultAsync(o => o.id == id);

            if (answer == null)
            {
                throw new NotFoundException("Icd10 not found");
            }

            return answer;
        }

        public async Task<List<Icd10DB>> GetIcd10RootsAsync()
        {

            List<Icd10DB>? Icd10 = await GetAllIcd10Async();

            Icd10 = Icd10.Where(o => o.parent == null).ToList();

            return Icd10;
        }

        public async Task<List<Icd10DB>> GetIcd10ByNameAndCode(string request)
        {

            List<Icd10DB>? Icd10 = await GetAllIcd10Async();


            Icd10 = Icd10.Where(o => Regex.Match(o.name, Regex.Escape(request), RegexOptions.IgnoreCase).Success ||
                                     Regex.Match(o.code, Regex.Escape(request), RegexOptions.IgnoreCase).Success).ToList();

            return Icd10;
        }

        public async Task ImportIcd10()
        {
            string pathToFile = Directory.GetCurrentDirectory() + _config.Value.Icd10FilePath;
            Icd10ImportListModel importModel = await _jsonReaderService.GetJsonDataAsync<Icd10ImportListModel>(pathToFile);

            List<Icd10HelpModel> listOfIcd10HelpModel = new List<Icd10HelpModel>();

            foreach (Icd10ImportModel icdImport in importModel.records)
            {
                listOfIcd10HelpModel.Add(new Icd10HelpModel()
                {
                    id = Guid.NewGuid(),
                    intId = icdImport.ID,
                    createTime = DateTime.Now,
                    code = icdImport.MKB_CODE,
                    name = icdImport.MKB_NAME,
                    parentIntId = icdImport.ID_PARENT == null ? null : int.Parse(icdImport.ID_PARENT)
                });
            }

            foreach (Icd10HelpModel icd10HelpModel in listOfIcd10HelpModel)
            {

                if (icd10HelpModel.parentIntId == null)
                {
                    icd10HelpModel.rootId = icd10HelpModel.id;
                    continue;
                }

                Icd10HelpModel? parentIcd10 = listOfIcd10HelpModel.FirstOrDefault(o => o.intId == icd10HelpModel.parentIntId);

                if (parentIcd10 == null)
                {
                    continue;
                }

                icd10HelpModel.parentId = parentIcd10.id;
            }

            List<Icd10HelpModel> listOfIcd10HelpModelsWithRoot = listOfIcd10HelpModel.Where(o => o.rootId != null).ToList();

            while (listOfIcd10HelpModelsWithRoot.Count() != listOfIcd10HelpModel.Count()) {

                foreach (var item in listOfIcd10HelpModelsWithRoot)
                {
                    List<Icd10HelpModel> icd10ToChange = listOfIcd10HelpModel.Where(o => o.parentId == item.id).ToList();

                    foreach (var item1 in icd10ToChange)
                    {
                        item1.rootId = item.rootId;
                    }
                }
                listOfIcd10HelpModelsWithRoot = listOfIcd10HelpModel.Where(o => o.rootId != null).ToList();
            }

            await _context.Icd10.ExecuteDeleteAsync();

            foreach (var item in listOfIcd10HelpModel)
            {
                await _context.Icd10.AddAsync(new Icd10DB()
                {
                    id = item.id,
                    code = item.code,
                    name = item.name,
                    createTime = item.createTime,
                    parentId = item.parentId,
                    rootId = (Guid)item.rootId
                });
            }

            await _context.SaveChangesAsync();

            return;
        }

        public Icd10RecordModel MapToIcd10RecordModel(Icd10DB icd10)
        {
            Icd10RecordModel icd10RecordModel = new Icd10RecordModel() {
                id = icd10.id,
                createTime = icd10.createTime,
                code = icd10.code,
                name = icd10.name
            };

            return icd10RecordModel;
        }


    }
}
