using Quartz.Impl;
using Quartz;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.EmailNotification;
using System.Data;

namespace Try_not_to_DIE.Services
{
    
    public class EmailSendingService
    {
        private readonly HospitalContext _context;

        public EmailSendingService(HospitalContext context)
        {
            _context = context;
        }

        public static async Task StartBackgroundTasks(IServiceProvider serviceProvider)
        {

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService<EmailJobFactory>();
            await scheduler.Start();

            IJobDetail jobDetail = JobBuilder.Create<GmailSender>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MailingTrigger", "default")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task addNewNotification(InspectionDB inspecion)
        {
            if (inspecion.nextVisitDate == null)
            {
                return;
            }
            InspectionDB? prevInspection = _context.Inspection.FirstOrDefault(o => o.id == inspecion.previousInspectionId);
            if (prevInspection != null)
            {
                DoctorNotification? notif = _context.DoctorNotification.FirstOrDefault(o => o.inspectionID == prevInspection.id);
                if (notif != null)
                {
                    await removeNotification(notif);
                }
            }
            _context.DoctorNotification.Add(new DoctorNotification()
            {
                inspectionID = inspecion.id,
                nextVisitDate = (DateTime)inspecion.nextVisitDate
            });
        }

        public async Task editNotification(InspectionDB inspecion)
        {
            DoctorNotification? notification = _context.DoctorNotification.FirstOrDefault(o => o.inspectionID == inspecion.id);
            if (notification == null)
            {
                if (inspecion.nextVisitDate != null)
                {
                    await addNewNotification(inspecion);
                }
                return;
            }

            notification.nextVisitDate = (DateTime)inspecion.nextVisitDate;
        }

        public async Task removeNotification(DoctorNotification notification)
        {
            _context.DoctorNotification.Remove(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DoctorNotification>> getAllOverdueNotifications()
        {
            return _context.DoctorNotification.Where(o => o.nextVisitDate <= DateTime.Now).ToList();
        }

        public async Task<InspectionDB> findInspectionByNotification(DoctorNotification notification)
        {
            return _context.Inspection.First(o => o.id == notification.inspectionID);
        }

    }
}
