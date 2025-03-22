using Microsoft.AspNetCore.Mvc;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Exceptions;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using static Try_not_to_DIE.Services.SpecialityService;

namespace Try_not_to_DIE.Services
{
    public class DoctorService
    {
        private readonly DoctorRepository _doctorRepository;

        public DoctorService(DoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<List<DoctorDB>> GetAllDoctorsAsync()
        {
            return await _doctorRepository.getList();
        }

        public async Task<DoctorDB> GetDoctorByIdAsync(Guid id)
        {
            DoctorDB? doctor = await _doctorRepository.getById(id);

            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }

            return doctor;
        }

        public async Task<DoctorDB> GetDoctorByEmailAsync(string email)
        {
            DoctorDB? doctor = await _doctorRepository.getByEmail(email);

            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }

            return doctor;
        }

        public async Task<DoctorDB> AddDoctorAsync(DoctorRegisterModel doctor, SpecialityModel speciality)
        {
            DoctorDB newDoctor = new DoctorDB() {
                id = new Guid(),
                createTime = DateTime.Now,
                name = doctor.name,
                birthday = doctor.birthday,
                gender = doctor.gender,
                email = doctor.email,
                phone = doctor.phone,
                speciality = speciality,
                password = doctor.password,
                isAuthorized = false
            };
            await _doctorRepository.create(newDoctor);
            await _doctorRepository.saveChanges();
            return newDoctor;
        }

        public async Task<DoctorDB> EditDoctorAsync(Guid doctorId, DoctorEditModel editedDoctor)
        {
            DoctorDB doctor = await GetDoctorByIdAsync(doctorId);
            doctor.name = editedDoctor.name;
            doctor.email = editedDoctor.email;
            doctor.birthday = editedDoctor.birthday;
            doctor.gender = editedDoctor.gender;
            doctor.phone = editedDoctor.phone;

            await _doctorRepository.saveChanges();

            return doctor;
        }

        public DoctorModel MapToDoctorModel(DoctorDB doctor)
        {
            DoctorModel answer = new DoctorModel() {
                id = doctor.id,
                createTime = doctor.createTime,
                name = doctor.name,
                birthday = doctor.birthday,
                gender = doctor.gender,
                email = doctor.email,
                phone = doctor.phone,
                specialityId = doctor.speciality.id
            };
            return answer;
        }
    }
}
