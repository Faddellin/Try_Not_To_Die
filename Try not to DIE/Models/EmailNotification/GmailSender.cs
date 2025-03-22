using Quartz;
using System.Net.Mail;
using System.Net;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Services;

namespace Try_not_to_DIE.Models.EmailNotification
{
    public class GmailSender : IJob
    {
        protected readonly IServiceScopeFactory serviceScopeFactory;

        public GmailSender(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            string fromMail = "forapitestskirillbelovodchenko@gmail.com";
            string fromPassword = "igfdhoyojrvoucvo";

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var _emailSendingService = scope.ServiceProvider.GetService<EmailSendingService>();

                var allNotifications = await _emailSendingService.getAllOverdueNotifications();

                foreach (var notification in allNotifications)
                {

                    InspectionDB inspection = await _emailSendingService.findInspectionByNotification(notification);

                    await sendEmail(fromMail,
                        "Patient " + inspection.patient.name + " visit",
                        "Your patient missed an inspection today in " + inspection.nextVisitDate.ToString(),
                        inspection.doctor.email,
                        fromPassword);

                    await _emailSendingService.removeNotification(notification);

                }
            }

        }
        public async Task sendEmail(string fromMail, string subject, string body, string toMail, string password)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                Body = body
            };
            mailMessage.To.Add(new MailAddress(toMail));

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, password),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);
        }

    }
}
