using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Try_not_to_DIE.Controllers;
using Try_not_to_DIE.Models.Authorization;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.ErrorResponse;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class PatientService
    {
        private readonly PatientRepository _patientRepository;


        public PatientService(PatientRepository patientRepository)
        {
            _patientRepository = patientRepository;

        }


        public async Task<List<PatientDB>> GetAllPatientsAsync()
        {
            return await _patientRepository.getList();
        }


        public async Task<PatientDB> GetPatientByIdAsync(Guid id)
        {
            PatientDB? answer = await _patientRepository.getById(id);

            if (answer == null)
            {
                throw new NotFoundException("Patient not found");
            }

            return answer;
        }


        public async Task<List<PatientDB>> GetPatientsByFiltersAsync(string name, [FromQuery] List<Conclusion> conclusions, PatientSorting? sorting, bool scheduledVisits
                                                    , bool onlyMine, DoctorDB doctor)
        {
            //фильтр по имени
            List<PatientDB> allPatientsFilteredByName = await GetAllPatientsAsync();

            allPatientsFilteredByName = allPatientsFilteredByName.Where(o => Regex.Match(o.name, name, RegexOptions.IgnoreCase).Success).ToList();
            //

            //фильтр по заключению
            List<PatientDB> patientsFilteredByConc = new List<PatientDB>();

            if (conclusions.Count() != 0)
            {
                foreach (var patient in allPatientsFilteredByName)
                {
                    var checkForConclusions = patient.allInspections.FirstOrDefault(o => o.patient == patient && conclusions.Contains(o.conclusion));

                    if (checkForConclusions != null)
                    {
                        patientsFilteredByConc.Add(patient);
                    }
                }
            }
            else
            {
                patientsFilteredByConc = allPatientsFilteredByName;
            }
            
            //

            //фильтр по запланированному осмотру
            List<PatientDB> patientsFilteredByShed = new List<PatientDB>();

            if (scheduledVisits == true)
            {
                foreach (var patient in patientsFilteredByConc)
                {
                    var checkForShed = patient.allInspections.FirstOrDefault(o => o.nextVisitDate != null && o.nextVisitDate > DateTime.Now);

                    if (checkForShed != null)
                    {
                        patientsFilteredByShed.Add(patient);
                    }
                }
            }
            else
            {
                patientsFilteredByShed = patientsFilteredByConc;
            }
            //

            //фильтр по id доктора
            List<PatientDB> patientsFilteredDocId = new List<PatientDB>();
            if (onlyMine == true)
            {
                foreach (var patient in patientsFilteredByShed)
                {
                    var checkForDocId = patient.allInspections.FirstOrDefault(o => o.doctor.id == doctor.id);

                    if (checkForDocId != null)
                    {
                        patientsFilteredDocId.Add(patient);
                    }
                }
            }
            else
            {
                patientsFilteredDocId = patientsFilteredByShed;
            }
            //

            
            //сортировка
            List<PatientDB> patientsSorted = new List<PatientDB>();
            switch (sorting)
            {
                case PatientSorting.NameAsc:
                    patientsSorted = patientsFilteredDocId.OrderBy(o => o.name).ToList();
                    break;
                case PatientSorting.NameDesc:
                    patientsSorted = patientsFilteredDocId.OrderByDescending(o => o.name).ToList();
                    break;
                case PatientSorting.CreateAsc:
                    patientsSorted = patientsFilteredDocId.OrderBy(o => o.createTime).ToList();
                    break;
                case PatientSorting.CreateDesc:
                    patientsSorted = patientsFilteredDocId.OrderByDescending(o => o.createTime).ToList();
                    break;
                case PatientSorting.InspectionAsc:
                    patientsSorted = patientsFilteredDocId.OrderBy(o => (o.allInspections.LastOrDefault() == null ? DateTime.Now : o.allInspections.Last().date)).ToList();
                    break;
                case PatientSorting.InspectionDesc:
                    patientsSorted = patientsFilteredDocId.OrderByDescending(o => (o.allInspections.LastOrDefault() == null ? DateTime.Now : o.allInspections.Last().date)).ToList();
                    break;
                default:
                    patientsSorted = patientsFilteredDocId;
                    break;
            }
            //

            return patientsSorted;
        }

        public async Task<List<InspectionDB>> GetPatientInspectionsByFiltersAsync(Guid patientId, [FromQuery] List<Guid> icdRoots, bool grouped)
        {

            PatientDB patient = await GetPatientByIdAsync(patientId);

            List<InspectionDB> inspectionsFiltered = patient.allInspections;

            if (icdRoots.Count() != 0)
            {
                inspectionsFiltered = inspectionsFiltered.Where(o => icdRoots.Contains(o.diagnoses.First().icd10.rootId)).ToList();
            }

            if (grouped == true)
            {
                inspectionsFiltered = inspectionsFiltered.Where(o => o.previousInspectionId == null).ToList();
            }

            return inspectionsFiltered;
        }

        public async Task<List<InspectionDB>> GetPatientInspectionsWithoutChildByRequestAsync(Guid patientId, string request)
        {

            PatientDB patient = await GetPatientByIdAsync(patientId);

            List<InspectionDB> inspectionswWithoutChild = patient.allInspections.Where(o => o.hasNested == false).ToList();

            List<InspectionDB> inspectionsFilteredByRequest = inspectionswWithoutChild.Where(o => o.diagnoses.Any(t =>
                                        Regex.Match(t.icd10.name, Regex.Escape(request), RegexOptions.IgnoreCase).Success ||
                                        Regex.Match(t.icd10.code, Regex.Escape(request), RegexOptions.IgnoreCase).Success)).ToList();

            return inspectionsFilteredByRequest;


        }

        public async Task<PatientDB> AddPatientAsync(PatientCreateModel patient)
        {
            PatientDB newPatient = new PatientDB() {
                id = new Guid(),
                createTime = DateTime.Now,
                name = patient.name,
                birthday = patient.birthday,
                gender = patient.gender,
                allInspections = new List<InspectionDB>()
            };
            await _patientRepository.create(newPatient);
            await _patientRepository.saveChanges();
            return newPatient;
        }

        public PatientModel MapToPatientModel(PatientDB patient)
        {
            PatientModel answer = new PatientModel() {
                id = patient.id,
                createTime = patient.createTime,
                name = patient.name,
                birthday = patient.birthday,
                gender = patient.gender
            };

            return answer;
        }
    }
}
