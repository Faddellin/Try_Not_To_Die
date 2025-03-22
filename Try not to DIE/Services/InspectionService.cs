using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Enumerables;

namespace Try_not_to_DIE.Services
{
    public class InspectionService
    {
        private readonly InspectionRepository _inspectionRepository;
        private readonly PatientService _patientService;
        private readonly DoctorService _doctorService;
        private readonly Icd10Service _icd10Service;
        private readonly DiagnosisService _diagnosisService;
        private readonly SpecialityService _specialityService;
        private readonly ConsultationService _consultationService;
        private readonly EmailSendingService _backgroundTasksService;

        public InspectionService(InspectionRepository inspectionRepository, PatientService patientService, DoctorService doctorService,
                                Icd10Service icd10Service, DiagnosisService diagnosisService, SpecialityService specialityService,
                                ConsultationService consultationService, EmailSendingService backgroundTasksService)
        {
            _inspectionRepository = inspectionRepository;
            _patientService = patientService;
            _doctorService = doctorService;
            _icd10Service = icd10Service;
            _diagnosisService = diagnosisService;
            _specialityService = specialityService;
            _consultationService = consultationService;
            _backgroundTasksService = backgroundTasksService;
        }

        public async Task<List<InspectionDB>> GetAllInspectionsAsync()
        {
            return await _inspectionRepository.getList();
        }



        public async Task<InspectionDB> GetInspectionByIdAsync(Guid id)
        {
            InspectionDB? inspection = await _inspectionRepository.getById(id);

            if (inspection == null)
            {
                throw new NotFoundException("Inspection not found");
            }

            return inspection;
        }

        public async Task<List<InspectionDB>> GetInspectionsByFiltersAsync(bool grouped, [FromQuery] List<Guid> icdRoots)
        {

            foreach (var id in icdRoots)
            {
                await _icd10Service.GetIcd10ByIdAsync(id);
            }

            List<InspectionDB> inspections = await GetAllInspectionsAsync();

            List<InspectionDB> inspectionsFilteredByGroups = new List<InspectionDB>();

            if (grouped)
            {
                inspectionsFilteredByGroups = inspections.Where(o => o.previousInspectionId == null).ToList();
            }
            else
            {
                List<InspectionDB> tempInspections = inspections.Where(o => o.previousInspectionId == null).ToList();

                foreach (var inspection in tempInspections)
                {
                    inspectionsFilteredByGroups.Add(inspection);

                    List<InspectionDB> thisInspectionChain = await GetInspectionChain(inspection.id);

                    inspectionsFilteredByGroups.AddRange(thisInspectionChain);

                }
            }

            List<InspectionDB> inspectionsFilteredByIcd = new List<InspectionDB>();

            if (icdRoots.Count == 0)
            {
                inspectionsFilteredByIcd = inspectionsFilteredByGroups;
            }
            else
            {
                inspectionsFilteredByIcd.AddRange(inspectionsFilteredByGroups.Where(o =>
                                                                                    icdRoots.Contains(o.diagnoses.First().icd10.rootId)));
            }

            return inspectionsFilteredByIcd;

        }

        public async Task<InspectionDB> AddInspectionAsync(InspectionCreateModel createModel, Guid patientId, Guid doctorId)
        {

            PatientDB patient = await _patientService.GetPatientByIdAsync(patientId);
            DoctorDB doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            InspectionDB? prevInspection = null;

            if (createModel.previousInspectionId != null)
            {
                prevInspection = await GetInspectionByIdAsync((Guid)createModel.previousInspectionId);

                if (prevInspection.patient.id != patient.id)
                {
                    throw new BadRequest("Previous inspection isn't this patient inspection");
                }

                if (createModel.date <= prevInspection.date || createModel.date > DateTime.Now)
                {
                    throw new BadRequest("Incorrect inspection date");
                }
            }
            if (createModel.diagnoses.Where(o => o.type == DiagnosisType.Main).Count() != 1)
            {
                throw new BadRequest("One diagnosis with the type Main must exist");
            }
            if (patient.allInspections.FirstOrDefault(o => o.conclusion == Conclusion.Death) != null)
            {
                throw new BadRequest("Patient already dead");
            }

            if (createModel.conclusion == Conclusion.Disease)
            {
                if (createModel.nextVisitDate == null || createModel.nextVisitDate < DateTime.Now)
                {
                    throw new BadRequest("Incorrect next visit date");
                }
            }
            else
            {
                if (createModel.nextVisitDate != null)
                {
                    throw new BadRequest("Patient cannot have next visit date");
                }
            }
            if (createModel.conclusion == Conclusion.Death)
            {
                if (createModel.deathDate == null || createModel.deathDate > DateTime.Now)
                {
                    throw new BadRequest("Incorrect death date");
                }
            }
            else
            {
                if (createModel.deathDate != null)
                {
                    throw new BadRequest("Patient cannot have a date of death");
                }
            }
            


            InspectionDB newInspection = new InspectionDB()
            {
                id = new Guid(),
                createTime = DateTime.Now,
                date = createModel.date,
                anamnesis = createModel.anamnesis,
                complaints = createModel.complaints,
                treatment = createModel.treatment,
                conclusion = createModel.conclusion,
                nextVisitDate = createModel.nextVisitDate,
                deathDate = createModel.deathDate,
                previousInspectionId = createModel.previousInspectionId,

                baseInspectionId = await FindBaseInspection(createModel.previousInspectionId),
                patient = patient,
                doctor = doctor,

                hasChain = false,
                hasNested = false
            };
            patient.allInspections.Add(newInspection);



            List<DiagnosisDB>? newDiagnoses = new List<DiagnosisDB>();
            foreach (var diagnosis in createModel.diagnoses)
            {
                Icd10DB? icd = _icd10Service.GetIcd10ByIdAsync(diagnosis.icdDiagnosisId).Result;

                DiagnosisDB? newDiagnos = await _diagnosisService.CreateDiagnosisAsync(diagnosis, icd);

                newDiagnoses.Add(newDiagnos);
            }
            newInspection.diagnoses = newDiagnoses;



            HashSet<Guid> alreadyExistedConsultations = new HashSet<Guid>(); 
            List<ConsultationDB>? newConsultations = new List<ConsultationDB>();
            foreach (var consultation in createModel.consultations)
            {

                if (alreadyExistedConsultations.Contains(consultation.specialityId))
                {
                    throw new BadRequest("Cannot be two consultations with the same speciality");
                }
                else
                {
                    alreadyExistedConsultations.Add(consultation.specialityId);
                }

                SpecialityModel? speciality = await _specialityService.GetSpecialityByIdAsync(consultation.specialityId);

                ConsultationDB newConsultation = await _consultationService.CreateConsultationAsync(consultation, newInspection, speciality, doctor);

                newConsultations.Add(newConsultation);
            }
            newInspection.consultations = newConsultations;



            await _inspectionRepository.create(newInspection);
            await _backgroundTasksService.addNewNotification(newInspection);
            await _inspectionRepository.saveChanges();

            if (prevInspection != null)
            {

                prevInspection.hasNested = true;
                prevInspection.nextInspectionId = newInspection.id;

                if (prevInspection.baseInspectionId == null)
                {
                    prevInspection.hasChain = true;
                }
            }

            await _inspectionRepository.saveChanges();

            return newInspection;
        }



        public async Task<InspectionDB?> EditInspectionAsync(Guid id, InspectionEditModel newInspection, DoctorDB doctor)
        {

            InspectionDB inspection = await GetInspectionByIdAsync(id);
            InspectionDB? inspectionWithDeathConclusion = inspection.patient.allInspections.FirstOrDefault(o => o.conclusion == Conclusion.Death);

            if (inspection.doctor.id != doctor.id)
            {
                throw new ForbiddenException("You are not the inspection author");
            }
            if (newInspection.diagnoses.Where(o => o.type == DiagnosisType.Main).Count() != 1)
            {
                throw new BadRequest("One diagnosis with the type Main must exist");
            }
            if (inspectionWithDeathConclusion != null && inspectionWithDeathConclusion != inspection
                && newInspection.conclusion == Conclusion.Death)
            {
                throw new BadRequest("Patient cannot have two inspections with Death conclusion");
            }

            if (newInspection.conclusion == Conclusion.Disease)
            {
                if (newInspection.nextVisitDate == null)
                {
                    throw new BadRequest("Incorrect next visit date");
                }
                if (inspection.nextVisitDate < DateTime.Now && newInspection.nextVisitDate > DateTime.Now)
                {
                    throw new BadRequest("Incorrect next visit date");
                }
                if (inspection.nextVisitDate > DateTime.Now && newInspection.nextVisitDate < DateTime.Now)
                {
                    throw new BadRequest("Incorrect next visit date");
                }
            }
            else
            {
                if (newInspection.nextVisitDate != null)
                {
                    throw new BadRequest("Patient cannot have next visit date");
                }
            }
            if (newInspection.conclusion == Conclusion.Death)
            {
                if (newInspection.deathDate == null || newInspection.deathDate > DateTime.Now)
                {
                    throw new BadRequest("Incorrect death date");
                }
            }
            else
            {
                if (newInspection.deathDate != null)
                {
                    throw new BadRequest("Patient cannot have a date of death");
                }
            }

            inspection.anamnesis = newInspection.anamnesis;
            inspection.complaints = newInspection.complaints;
            inspection.treatment = newInspection.treatment;
            inspection.conclusion = newInspection.conclusion;
            inspection.deathDate = newInspection.deathDate;
            inspection.nextVisitDate = newInspection.nextVisitDate;

            for (int i = 0; i < inspection.diagnoses.Count(); i++)
            {
                Icd10DB? icd = await _icd10Service.GetIcd10ByIdAsync(newInspection.diagnoses[i].icdDiagnosisId);

                DiagnosisDB diagnosis = inspection.diagnoses[i];

                diagnosis.icd10 = icd;
                diagnosis.type = newInspection.diagnoses[i].type;
                diagnosis.description = newInspection.diagnoses[i].description;

            }

            await _backgroundTasksService.editNotification(inspection);

            await _inspectionRepository.saveChanges();

            return inspection;
        }

        public async Task<List<InspectionDB>> GetInspectionChain(Guid id)
        {
            InspectionDB thisInspection = await GetInspectionByIdAsync(id);

            List<InspectionDB> chain = thisInspection.patient.allInspections.Where(o => o.baseInspectionId == thisInspection.id).ToList();

            List<InspectionDB> filteredChain = new List<InspectionDB>();

            InspectionDB curInspection = thisInspection;

            while (curInspection.nextInspectionId != null)
            {
                curInspection = chain.First(o => o.id == curInspection.nextInspectionId);
                filteredChain.Add(curInspection);

            }

            return filteredChain;
        }


        public InspectionModel MapToInspectionModel(InspectionDB inspection)
        {
            InspectionModel answer = new InspectionModel()
            {
                id = inspection.id,
                createTime = inspection.createTime,
                date = inspection.date,
                anamnesis = inspection.anamnesis,
                complaints = inspection.complaints,
                treatment = inspection.treatment,
                conclusion = inspection.conclusion,
                nextVisitDate = inspection.nextVisitDate,
                deathDate = inspection.deathDate,

                previousInspectionId = inspection.previousInspectionId,
                baseInspectionId = inspection.baseInspectionId,
                patient = _patientService.MapToPatientModel(inspection.patient),
                doctor = _doctorService.MapToDoctorModel(inspection.doctor),

                diagnoses = new List<DiagnosisModel>(),
                consultations = new List<InspectionConsultationModel>()
            };
            
            foreach (var diagnos in inspection.diagnoses)
            {
                answer.diagnoses.Add(_diagnosisService.MapToDiagnosisModel(diagnos));
            }

            foreach (var item in inspection.consultations)
            {
                answer.consultations.Add(_consultationService.MapToInspectionConsultationModel(item));
            }

            return answer;
        }

        public InspectionPreviewModel MapToInspectionPreviewModel(InspectionDB inspection)
        {
            InspectionPreviewModel answer = new InspectionPreviewModel()
            {
                id = inspection.id,
                createTime = inspection.createTime,
                previousId = inspection.previousInspectionId,
                date = inspection.date,
                conclusion = inspection.conclusion,
                doctorId = inspection.doctor.id,
                doctor = inspection.doctor.name,
                patientId = inspection.patient.id,
                patient = inspection.patient.name,
                diagnosis = _diagnosisService.MapToDiagnosisModel(inspection.diagnoses.First()),

                hasNested = inspection.hasNested,
                hasChain = inspection.hasChain
            };

            return answer;
        }

        public InspectionShortModel MapToInspectionShortModel(InspectionDB inspection)
        {
            InspectionShortModel answer = new InspectionShortModel()
            {
                id = inspection.id,
                createTime = inspection.createTime,
                date = inspection.date,
                diagnosis = _diagnosisService.MapToDiagnosisModel(inspection.diagnoses.First())
            };

            return answer;
        }

        private async Task<Guid?> FindBaseInspection(Guid? prevInspectionId)
        {
            if (prevInspectionId != null)
            {
                InspectionDB prevInspection = await GetInspectionByIdAsync((Guid)prevInspectionId);

                if (prevInspection.baseInspectionId == null)
                {
                    return prevInspectionId;
                }
                else
                {
                    return prevInspection.baseInspectionId;
                }
            }
            else
            {
                return null;
            }
        }


    }
}
