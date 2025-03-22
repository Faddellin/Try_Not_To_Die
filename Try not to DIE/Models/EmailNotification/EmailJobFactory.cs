using Quartz.Spi;
using Quartz;

namespace Try_not_to_DIE.Models.EmailNotification
{
    public class EmailJobFactory : IJobFactory
    {
        protected readonly IServiceScopeFactory serviceScopeFactory;

        public EmailJobFactory(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var job = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job;
            }

        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
