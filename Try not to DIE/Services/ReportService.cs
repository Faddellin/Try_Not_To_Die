using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Enumerables;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class ReportService
    {
        private readonly CommentRepository _commentRepository;
        private readonly DoctorService _doctorService;
        private readonly InspectionService _inspectionService;
        private readonly Icd10Service _icd10Service;
        private readonly PatientService _patientService;

        public ReportService(CommentRepository commentRepository, DoctorService doctorService, InspectionService inspectionService, Icd10Service icd10Service
                            , PatientService patientService)
        {
            _commentRepository = commentRepository;
            _doctorService = doctorService;
            _inspectionService = inspectionService;
            _icd10Service = icd10Service;
            _patientService = patientService;   
        }

        private async Task<List<InspectionDB>> GetInspectionsByFilters(DateTime start, DateTime end, [FromQuery] List<Guid> icdRoots)
        {

            List<InspectionDB> inspections = await _inspectionService.GetAllInspectionsAsync();

            List<InspectionDB>? allInspectionsFilteredByTime = inspections.Where(o => o.date < end && o.date > start).ToList();


            List<InspectionDB>? allInspectionsFilteredByIcd = allInspectionsFilteredByTime;
            if (icdRoots.Count() != 0)
            {
                allInspectionsFilteredByIcd = allInspectionsFilteredByIcd.Where(
                            o => icdRoots.Contains(o.diagnoses.First(o => o.type == DiagnosisType.Main).icd10.rootId)).ToList();
            }

            return allInspectionsFilteredByIcd;
        }


        private IcdRootsReportRecordModel CreateIcdRootsReportRecordModel(PatientDB patient, Dictionary<string, int> visitsByRoot)
        {
            IcdRootsReportRecordModel model = new IcdRootsReportRecordModel() {
                patientName = patient.name,
                patientBirthdate = patient.birthday,
                gender = patient.gender,
                visitsByRoot = visitsByRoot
            };

            return model;
        }

        private IcdRootsReportFiltersModel CreateIcdRootsReportFiltersModel(DateTime start, DateTime end, List<string> icdRoots)
        {
            IcdRootsReportFiltersModel model = new IcdRootsReportFiltersModel()
            {
                start = start,
                end = end,
                icdRoots = icdRoots
            };

            return model;
        }

        public async Task<IcdRootsReportModel> CreateIcdRootsReportModel(DateTime start, DateTime end, List<Guid> icdRoots)
        {
            
            List<InspectionDB> inspectionsFiltered = await GetInspectionsByFilters(start, end, icdRoots);

            Dictionary<Guid, Dictionary<string, int>> allPatientsIcdCount = new Dictionary<Guid, Dictionary<string, int>>();
            Dictionary<string, int> eachIcdCount = new Dictionary<string, int>();
            CalculateVisitsByIcdRootForEachPatient(ref allPatientsIcdCount, ref eachIcdCount, inspectionsFiltered);


            List<string> icdRootsCodes = new List<string>();
            foreach (var item in icdRoots)
            {
                Icd10DB icd10 = await _icd10Service.GetIcd10ByIdAsync(item);
                icdRootsCodes.Add(icd10.code);
            }


            List<IcdRootsReportRecordModel> records = new List<IcdRootsReportRecordModel>();
            foreach (var item in allPatientsIcdCount)
            {
                PatientDB patient = await _patientService.GetPatientByIdAsync(item.Key);

                records.Add(CreateIcdRootsReportRecordModel(patient, item.Value));
            }


            IcdRootsReportModel model = new IcdRootsReportModel()
            {
                filters = CreateIcdRootsReportFiltersModel(start,end,icdRootsCodes),
                records = records,
                summaryByRoot = eachIcdCount
            };

            return model;
        }

        private void CalculateVisitsByIcdRootForEachPatient(ref Dictionary<Guid, Dictionary<string, int>> allPatientsIcdCount,
                                                ref Dictionary<string, int> eachIcdCount, List<InspectionDB> inspections)
        {

            Dictionary<Guid, Dictionary<string, int>> _allPatientsIcdCount = new Dictionary<Guid, Dictionary<string, int>>();

            Dictionary<string, int> _eachIcdCount = new Dictionary<string, int>();

            foreach (var inspection in inspections)
            {

                if (!_allPatientsIcdCount.ContainsKey(inspection.patient.id))
                {
                    _allPatientsIcdCount.Add(inspection.patient.id, new Dictionary<string, int>());
                }

                Dictionary<string, int> patientDictionary = _allPatientsIcdCount[inspection.patient.id];
                string icd10Code = inspection.diagnoses[0].icd10.code;

                if (!patientDictionary.ContainsKey(icd10Code))
                {
                    patientDictionary.Add(icd10Code, 0);
                }
                patientDictionary[icd10Code]++;


                if (!_eachIcdCount.ContainsKey(icd10Code))
                {
                    _eachIcdCount.Add(icd10Code, 0);
                }
                _eachIcdCount[icd10Code]++;
            }

            allPatientsIcdCount = _allPatientsIcdCount;
            eachIcdCount = _eachIcdCount;

        }
    }
}
