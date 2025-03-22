using Microsoft.AspNetCore.Mvc;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class DiagnosisService
    {
        private readonly DiagnosRepository _diagnosisyRepository;

        public DiagnosisService(DiagnosRepository diagnosisyRepository)
        {
            _diagnosisyRepository = diagnosisyRepository;
        }

        public async Task<List<DiagnosisDB>> GetAllDiagnosisAsync()
        {
            return await _diagnosisyRepository.getList();
        }

        public async Task<DiagnosisDB?> GetDiagnosisByIdAsync(Guid id)
        {
            return await _diagnosisyRepository.getById(id);
        }

        public async Task<DiagnosisDB> CreateDiagnosisAsync(DiagnosisCreateModel newDiagnosis, Icd10DB icd)
        {
            return await _diagnosisyRepository.create(new DiagnosisDB()
            {
                id = new Guid(),
                createTime = DateTime.Now,
                icd10 = icd,
                description = newDiagnosis.description,
                type = newDiagnosis.type
            });
        }

        public async Task<DiagnosisDB> EditDiagnosisAsync(DiagnosisDB oldDiagnos, DiagnosisCreateModel newDiagnosis, Icd10DB icd)
        {
            DiagnosisDB? diagnos = await _diagnosisyRepository.getById(oldDiagnos.id);

            if (diagnos == null)
            {
                throw new NotFoundException("Diagnosis not found");
            }

            diagnos.icd10 = icd;
            diagnos.description = newDiagnosis.description;
            diagnos.type = newDiagnosis.type;

            await _diagnosisyRepository.saveChanges();

            return diagnos;
        }

        public DiagnosisModel MapToDiagnosisModel(DiagnosisDB diagnos)
        {
            DiagnosisModel model = new DiagnosisModel() {
                id = diagnos.id,
                createTime = diagnos.createTime,
                code = diagnos.icd10.code,
                name = diagnos.icd10.name,
                description = diagnos.description,
                type = diagnos.type
            };
            return model;
            
        }
    }
}
