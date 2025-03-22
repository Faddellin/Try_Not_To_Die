using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models;
using Try_not_to_DIE.Models.EmailNotification;
using Microsoft.EntityFrameworkCore;

namespace Try_not_to_DIE.DBContext
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options) {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }

        public DbSet<PatientDB> Patient { get; set; }

        public DbSet<InspectionDB> Inspection { get; set; }

        public DbSet<DoctorDB> Doctor { get; set; }

        public DbSet<ConsultationDB> Consultation { get; set; }

        public DbSet<CommentDB> Comment { get; set; }

        public DbSet<SpecialityModel> Speciality { get; set; }

        public DbSet<DiagnosisDB> Diagnosis {  get; set; }

        public DbSet<Icd10DB> Icd10 { get; set; }

        public DbSet<DoctorNotification> DoctorNotification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorNotification>().HasKey(o => o.inspectionID);
        }
    }
}
