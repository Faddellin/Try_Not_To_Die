using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Models.Exceptions;

namespace Try_not_to_DIE.Services
{
    public class PageService
    {

        private readonly Icd10Service _icd10Service;
        private readonly PatientService _patientService;
        private readonly InspectionService _inspectionService;

        public PageService(Icd10Service icd10Service, PatientService patientService, InspectionService inspectionService)
        {
            _icd10Service = icd10Service;
            _patientService = patientService;
            _inspectionService = inspectionService;
        }

        public PageInfoModel CreatePagedListModel(int size, int current, int countOfModels)
        {

            PageInfoModel newPageInfoModel = new PageInfoModel() {
                size = size,
                current = current,
                count = countOfModels / size,
            };

            if (countOfModels % size > 0)
            {
                newPageInfoModel.count++;
            }

            return newPageInfoModel;
        }

        public SpecialtiesPagedListModel CreateSpecialtiesPagedListModel(int size, int page, List<SpecialityModel> specialties)
        {
            int startIndex = (page - 1) * size;

            if (startIndex >= specialties.Count())
            {
                throw new BadRequest("Invalid value for attribute page");
            }

            int endNumber = (specialties.Count() >= startIndex + size) ? startIndex + size : specialties.Count();

            SpecialtiesPagedListModel specialtiesPagedListModel = new SpecialtiesPagedListModel() {

                specialties = new List<SpecialityModel>(),
                pagination = CreatePagedListModel(size, page, specialties.Count())
            };

            for (int i = startIndex; i < endNumber; i++)
            {
                specialtiesPagedListModel.specialties.Add(specialties[i]);
            }

            return specialtiesPagedListModel;
        }

        public Icd10SearchModel CreateIcd10SearchModel(int size, int page, List<Icd10DB> records)
        {
            int startIndex = (page - 1) * size;

            if (startIndex >= records.Count())
            {
                throw new BadRequest("Invalid value for attribute page");
            }

            int endNumber = (records.Count() >= startIndex + size) ? startIndex + size : records.Count();

            Icd10SearchModel icd10SearchModel = new Icd10SearchModel() {
                records = new List<Icd10RecordModel>(),
                pagination = CreatePagedListModel(size, page, records.Count())
            };
            

            for (int i = startIndex; i < endNumber; i++)
            {
                icd10SearchModel.records.Add(_icd10Service.MapToIcd10RecordModel(records[i]));
            }

            return icd10SearchModel;
        }

        public PatientPagedListModel CreatePatientPagedListModel(int size, int page, List<PatientDB> patients)
        {
            int startIndex = (page - 1) * size;
            int endNumber = (patients.Count() >= startIndex + size) ? startIndex + size : patients.Count();

            PatientPagedListModel patientPagedListModel = new PatientPagedListModel() {
                patients = new List<PatientModel>(),
                pagination = CreatePagedListModel(size, page, patients.Count())
            };
            

            for (int i = startIndex; i < endNumber; i++)
            {
                patientPagedListModel.patients.Add(_patientService.MapToPatientModel(patients[i]));
            }

            return patientPagedListModel;
        }

        public InspectionPagedListModel CreateInspectionPagedListModel(int size, int page, List<InspectionDB> patients)
        {
            int startIndex = (page - 1) * size;
            int endNumber = (patients.Count() >= startIndex + size) ? startIndex + size : patients.Count();

            InspectionPagedListModel inspectionPagedListModel = new InspectionPagedListModel()
            {
                inspections = new List<InspectionPreviewModel>(),
                pagination = CreatePagedListModel(size, page, patients.Count())
            };


            for (int i = startIndex; i < endNumber; i++)
            {
                inspectionPagedListModel.inspections.Add(_inspectionService.MapToInspectionPreviewModel(patients[i]));
            }

            return inspectionPagedListModel;
        }
    }
}
